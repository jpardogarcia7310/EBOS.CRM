using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Application.Shared.Audit;
using EBOS.CRM.Application.Shared.Observability;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Domain.Exceptions;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Lead.Commands.UpdateLead;

public class UpdateLeadCommandHandler(ILeadRepository repository, IAuditService auditService,
    ICurrentUserContext currentUser, IMapper mapper, IDomainOperationalEventPublisher? domainOperationalEventPublisher = null) : IRequestHandler<UpdateLeadCommand, LeadResponse?>
{
    public async Task<LeadResponse?> Handle(UpdateLeadCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entityRequest = request.LeadRequest ?? throw new ArgumentNullException(nameof(request.LeadRequest));
        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null)
        {
            return null;
        }
        if (entity.TenantId != entityRequest.TenantId)
        {
            throw new DomainConflictException("Lead tenant mismatch.", "DOMAIN_CONFLICT_LEAD_TENANT_MISMATCH");
        }

        var oldValues = AuditSerialization.Serialize(entity);
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
            await repository.UpdateAsync(entity, cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);

            var auditRequest = new AuditInsertRequest(
                UserId: currentUser.UserId,
                TimeStamp: DateTimeOffset.UtcNow,
                Action: AuditActions.Update,
                Entity: nameof(Domain.Entities.CRM.Lead),
                RegisterId: entity.Id,
                OldValues: oldValues,
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
