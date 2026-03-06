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

namespace EBOS.CRM.Application.Features.CRM.Service.Case.Commands.AssignCaseQueue;

public class AssignCaseQueueCommandHandler(
    ICaseRepository repository,
    IQueueRepository queueRepository,
    IAuditService auditService,
    ICurrentUserContext currentUser,
    IMapper mapper,
    ICaseReferenceValidationService? caseReferenceValidationService = null,
    IDomainOperationalEventPublisher? domainOperationalEventPublisher = null) : IRequestHandler<AssignCaseQueueCommand, CaseResponse?>
{
    public async Task<CaseResponse?> Handle(AssignCaseQueueCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entityRequest = request.CaseRequest ?? throw new ArgumentNullException(nameof(request.CaseRequest));
        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null)
        {
            return null;
        }

        if (entity.ClosedAt.HasValue)
        {
            throw new DomainRuleViolationException("Cannot change queue for a closed case.", "DOMAIN_RULE_VIOLATION_CASE_CLOSED_QUEUE_CHANGE");
        }

        if (caseReferenceValidationService is not null)
        {
            _ = await caseReferenceValidationService.EnsureQueueAvailableAsync(entity.TenantId, entityRequest.QueueId, cancellationToken);
        }
        else
        {
            var queue = await queueRepository.GetByIdAsync(entityRequest.QueueId, cancellationToken)
                ?? throw new DomainValidationException("Queue not found.", "DOMAIN_VALIDATION_QUEUE_NOT_FOUND");
            if (!queue.IsActive)
            {
                throw new DomainRuleViolationException("Queue is not active.", "DOMAIN_RULE_VIOLATION_QUEUE_INACTIVE");
            }
            if (queue.TenantId != entity.TenantId)
            {
                throw new DomainConflictException("Queue tenant mismatch.", "DOMAIN_CONFLICT_QUEUE_TENANT_MISMATCH");
            }
        }

        var oldValues = AuditSerialization.Serialize(entity);
        entity.AssignQueue(entityRequest.QueueId);

        await repository.BeginTransactionAsync(cancellationToken);

        try
        {
            await repository.UpdateAsync(entity, cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);

            var auditRequest = new AuditInsertRequest(
                UserId: currentUser.UserId,
                TimeStamp: DateTimeOffset.UtcNow,
                Action: AuditActions.Update,
                Entity: nameof(Domain.Entities.CRM.Case),
                RegisterId: entity.Id,
                OldValues: oldValues,
                NewValues: AuditSerialization.Serialize(entity),
                CorrelationId: currentUser.CorrelationId);

            await auditService.InsertAuditAsync(auditRequest, cancellationToken);
            if (domainOperationalEventPublisher is not null)
            {
                await domainOperationalEventPublisher.PublishAsync(
                    nameof(Domain.Entities.CRM.Case),
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

        return mapper.Map<CaseResponse>(entity);
    }
}


