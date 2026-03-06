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

namespace EBOS.CRM.Application.Features.CRM.Opportunity.Commands.UpdateOpportunity;

public class UpdateOpportunityCommandHandler(IOpportunityRepository repository, IAuditService auditService,
    ICurrentUserContext currentUser, IMapper mapper, IOpportunityStageValidationService stageValidationService,
    IDomainOperationalEventPublisher? domainOperationalEventPublisher = null)
    : IRequestHandler<UpdateOpportunityCommand, OpportunityResponse?>
{
    public async Task<OpportunityResponse?> Handle(UpdateOpportunityCommand request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entityRequest = request.OpportunityRequest ??
                            throw new ArgumentNullException(nameof(request.OpportunityRequest));
        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null)
        {
            return null;
        }
        if (entity.TenantId != entityRequest.TenantId)
        {
            throw new DomainConflictException("Opportunity tenant mismatch.", "DOMAIN_CONFLICT_OPPORTUNITY_TENANT_MISMATCH");
        }

        var oldValues = AuditSerialization.Serialize(entity);
        await stageValidationService.EnsureStageAvailableAsync(
            entityRequest.TenantId,
            entityRequest.StageId,
            cancellationToken);
        entity.ApplyUpdate(
            entityRequest.Name,
            entityRequest.StageId,
            entityRequest.OwnerUserId,
            entityRequest.CustomerId,
            entityRequest.ExpectedCloseDate,
            entityRequest.Amount,
            entityRequest.Probability,
            entityRequest.Source,
            entityRequest.SourceLeadId,
            entityRequest.CloseReason);

        await repository.BeginTransactionAsync(cancellationToken);

        try
        {
            await repository.UpdateAsync(entity, cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);

            var auditRequest = new AuditInsertRequest(
                UserId: currentUser.UserId,
                TimeStamp: DateTimeOffset.UtcNow,
                Action: AuditActions.Update,
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

        return mapper.Map<OpportunityResponse>(entity);
    }
}
