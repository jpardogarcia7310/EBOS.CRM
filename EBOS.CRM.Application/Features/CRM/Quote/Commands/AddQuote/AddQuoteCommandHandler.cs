using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Application.Shared.Audit;
using EBOS.CRM.Application.Shared.Observability;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Domain.Exceptions;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services;
using EBOS.CRM.Domain.Interfaces.Services.CRM;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Quote.Commands.AddQuote;

public class AddQuoteCommandHandler(IQuoteRepository repository, IAuditService auditService,
    ICurrentUserContext currentUser, IMapper mapper, IQuoteOpportunityValidationService quoteOpportunityValidationService,
    IDomainOperationalEventPublisher? domainOperationalEventPublisher = null) : IRequestHandler<AddQuoteCommand, QuoteResponse>
{
    public async Task<QuoteResponse> Handle(AddQuoteCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entityRequest = request.QuoteRequest ?? throw new ArgumentNullException(nameof(request.QuoteRequest));
        await quoteOpportunityValidationService.EnsureOpportunityAvailableAsync(
            entityRequest.TenantId,
            entityRequest.OpportunityId,
            cancellationToken);
        var entity = mapper.Map<global::EBOS.CRM.Domain.Entities.CRM.Quote>(entityRequest);
        entity.ApplyUpdate(
            entityRequest.OpportunityId,
            entityRequest.Status,
            entityRequest.ReferenceNumber,
            entityRequest.SubtotalAmount,
            entityRequest.DiscountAmount,
            entityRequest.TotalAmount,
            entityRequest.ValidUntil,
            entityRequest.Notes);

        await repository.BeginTransactionAsync(cancellationToken);

        try
        {
            await repository.AddAsync(entity, cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);

            var auditRequest = new AuditInsertRequest(
                UserId: currentUser.UserId,
                TimeStamp: DateTimeOffset.UtcNow,
                Action: AuditActions.Add,
                Entity: nameof(Domain.Entities.CRM.Quote),
                RegisterId: entity.Id,
                OldValues: null,
                NewValues: AuditSerialization.Serialize(entity),
                CorrelationId: currentUser.CorrelationId);

            await auditService.InsertAuditAsync(auditRequest, cancellationToken);
            if (domainOperationalEventPublisher is not null)
            {
                await domainOperationalEventPublisher.PublishAsync(
                    nameof(Domain.Entities.CRM.Quote),
                    entity.Id,
                    entity.DequeueOperationalEvents(),
                    cancellationToken);
            }
            await repository.CommitAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            await repository.RollbackAsync(cancellationToken);

            if (DomainTransientFailureClassifier.TryClassify(ex, nameof(Handle), out var transient))
            {
                throw transient;
            }

            throw;
        }

        return mapper.Map<QuoteResponse>(entity);
    }
}
