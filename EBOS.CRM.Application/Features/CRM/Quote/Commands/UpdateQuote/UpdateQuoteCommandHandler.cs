using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Application.Shared.Audit;
using EBOS.CRM.Application.Shared.Observability;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Domain.Exceptions;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Quote.Commands.UpdateQuote;

public class UpdateQuoteCommandHandler(IQuoteRepository repository, IAuditService auditService,
    ICurrentUserContext currentUser, IMapper mapper, IDomainOperationalEventPublisher? domainOperationalEventPublisher = null) : IRequestHandler<UpdateQuoteCommand, QuoteResponse?>
{
    public async Task<QuoteResponse?> Handle(UpdateQuoteCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entityRequest = request.QuoteRequest ?? throw new ArgumentNullException(nameof(request.QuoteRequest));
        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null)
        {
            return null;
        }
        if (entity.TenantId != entityRequest.TenantId)
        {
            throw new DomainConflictException("Quote tenant mismatch.", "DOMAIN_CONFLICT_QUOTE_TENANT_MISMATCH");
        }

        var oldValues = AuditSerialization.Serialize(entity);
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
            await repository.UpdateAsync(entity, cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);

            var auditRequest = new AuditInsertRequest(
                UserId: currentUser.UserId,
                TimeStamp: DateTimeOffset.UtcNow,
                Action: AuditActions.Update,
                Entity: nameof(Domain.Entities.CRM.Quote),
                RegisterId: entity.Id,
                OldValues: oldValues,
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
        catch
        {
            await repository.RollbackAsync(cancellationToken);
            throw;
        }

        return mapper.Map<QuoteResponse>(entity);
    }
}
