using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Application.Shared.Audit;
using EBOS.CRM.Application.Shared.Observability;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Domain.Exceptions;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services;
using EBOS.CRM.Domain.Interfaces.Services.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Opportunity.Commands.CloseOpportunity;

public class CloseOpportunityCommandHandler(IOpportunityRepository repository, IAuditService auditService,
    ICurrentUserContext currentUser, IOpportunityStageValidationService stageValidationService,
    IDomainOperationalEventPublisher? domainOperationalEventPublisher = null) : IRequestHandler<CloseOpportunityCommand, OpportunityResponse?>
{
    public async Task<OpportunityResponse?> Handle(CloseOpportunityCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null)
        {
            return null;
        }
        if (entity.TenantId != request.OpportunityRequest.TenantId)
        {
            throw new DomainConflictException("Opportunity tenant mismatch.", "DOMAIN_CONFLICT_OPPORTUNITY_TENANT_MISMATCH");
        }

        var oldValues = AuditSerialization.Serialize(entity);
        await stageValidationService.EnsureStageAvailableAsync(
            request.OpportunityRequest.TenantId,
            request.OpportunityRequest.StageId,
            cancellationToken);
        entity.Close(
            request.OpportunityRequest.StageId,
            request.OpportunityRequest.IsWon,
            request.OpportunityRequest.CloseReason);

        await repository.BeginTransactionAsync(cancellationToken);

        try
        {
            await repository.UpdateAsync(entity, cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);

            var auditRequest = new AuditInsertRequest(
                UserId: currentUser.UserId,
                TimeStamp: DateTimeOffset.UtcNow,
                Action: AuditActions.Patch,
                Entity: nameof(Domain.Entities.CRM.Opportunity),
                RegisterId: entity.Id,
                OldValues: oldValues,
                NewValues: AuditSerialization.Serialize(entity),
                CorrelationId: currentUser.CorrelationId);

            await auditService.InsertAuditAsync(auditRequest, cancellationToken);
            if (domainOperationalEventPublisher is not null)
            {
                await domainOperationalEventPublisher.PublishAsync(
                    nameof(Domain.Entities.CRM.Opportunity),
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

        return new OpportunityResponse(
            entity.Id,
            entity.TenantId,
            entity.Name,
            entity.StageId,
            entity.OwnerUserId,
            entity.CustomerId,
            entity.ExpectedCloseDate,
            entity.Amount,
            entity.Probability,
            entity.Source,
            entity.SourceLeadId,
            entity.CloseReason,
            !entity.Erased);
    }
}
