using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Application.Shared.Audit;
using EBOS.CRM.Application.Shared.Observability;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Lead.Commands.AddLead;

public class AddLeadCommandHandler(ILeadRepository repository, IAuditService auditService,
    ICurrentUserContext currentUser, IMapper mapper, IDomainOperationalEventPublisher? domainOperationalEventPublisher = null) : IRequestHandler<AddLeadCommand, LeadResponse>
{
    public async Task<LeadResponse> Handle(AddLeadCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entityRequest = request.LeadRequest ?? throw new ArgumentNullException(nameof(request.LeadRequest));
        var entity = mapper.Map<global::EBOS.CRM.Domain.Entities.CRM.Lead>(entityRequest);
        entity.ApplyUpdate(
            entityRequest.Source,
            entityRequest.Status,
            entityRequest.OwnerUserId,
            entityRequest.CompanyName,
            entityRequest.ContactName,
            entityRequest.Email,
            entityRequest.Phone,
            entityRequest.EstimatedValue,
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
                Entity: nameof(Domain.Entities.CRM.Lead),
                RegisterId: entity.Id,
                OldValues: null,
                NewValues: AuditSerialization.Serialize(entity),
                CorrelationId: currentUser.CorrelationId);

            await auditService.InsertAuditAsync(auditRequest, cancellationToken);
            if (domainOperationalEventPublisher is not null)
            {
                await domainOperationalEventPublisher.PublishAsync(
                    nameof(Domain.Entities.CRM.Lead),
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

        return mapper.Map<LeadResponse>(entity);
    }
}
