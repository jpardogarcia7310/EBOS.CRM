using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Application.Shared.Audit;
using EBOS.CRM.Application.Shared.Observability;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Domain.Exceptions;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Lead.Commands.DisqualifyLead;

public class DisqualifyLeadCommandHandler(ILeadRepository repository, IAuditService auditService,
    ICurrentUserContext currentUser, IDomainOperationalEventPublisher? domainOperationalEventPublisher = null) : IRequestHandler<DisqualifyLeadCommand, LeadResponse?>
{
    public async Task<LeadResponse?> Handle(DisqualifyLeadCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null)
        {
            return null;
        }
        if (entity.TenantId != request.LeadRequest.TenantId)
        {
            throw new DomainConflictException("Lead tenant mismatch.", "DOMAIN_CONFLICT_LEAD_TENANT_MISMATCH");
        }

        var oldValues = AuditSerialization.Serialize(entity);
        entity.Disqualify(request.LeadRequest.Reason);

        await repository.BeginTransactionAsync(cancellationToken);

        try
        {
            await repository.UpdateAsync(entity, cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);

            var auditRequest = new AuditInsertRequest(
                UserId: currentUser.UserId,
                TimeStamp: DateTimeOffset.UtcNow,
                Action: AuditActions.Patch,
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
        catch (Exception ex)
        {
            await repository.RollbackAsync(cancellationToken);

            if (DomainTransientFailureClassifier.TryClassify(ex, nameof(Handle), out var transient))
            {
                throw transient;
            }

            throw;
        }

        return new LeadResponse(
            entity.Id,
            entity.TenantId,
            entity.Source,
            entity.Status,
            entity.OwnerUserId,
            entity.CompanyName,
            entity.ContactName,
            entity.Email,
            entity.Phone,
            entity.EstimatedValue,
            entity.Notes,
            entity.ConvertedOpportunityId,
            !entity.Erased);
    }
}
