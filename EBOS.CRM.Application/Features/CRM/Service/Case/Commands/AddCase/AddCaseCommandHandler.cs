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

namespace EBOS.CRM.Application.Features.CRM.Service.Case.Commands.AddCase;

public class AddCaseCommandHandler(
    ICaseRepository repository,
    IQueueRepository queueRepository,
    ISlaRepository slaRepository,
    IAuditService auditService,
    ICurrentUserContext currentUser,
    IMapper mapper,
    ICaseReferenceValidationService? caseReferenceValidationService = null,
    IDomainOperationalEventPublisher? domainOperationalEventPublisher = null) : IRequestHandler<AddCaseCommand, CaseResponse>
{
    public async Task<CaseResponse> Handle(AddCaseCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entityRequest = request.CaseRequest ?? throw new ArgumentNullException(nameof(request.CaseRequest));
        global::EBOS.CRM.Domain.Entities.CRM.Sla sla;
        if (caseReferenceValidationService is not null)
        {
            _ = await caseReferenceValidationService.EnsureQueueAvailableAsync(entityRequest.TenantId, entityRequest.QueueId, cancellationToken);
            sla = await caseReferenceValidationService.EnsureSlaAvailableAsync(entityRequest.TenantId, entityRequest.SlaId, cancellationToken);
        }
        else
        {
            var queue = await queueRepository.GetByIdAsync(entityRequest.QueueId, cancellationToken)
                ?? throw new DomainValidationException("Queue not found.", "DOMAIN_VALIDATION_QUEUE_NOT_FOUND");
            if (!queue.IsActive)
            {
                throw new DomainRuleViolationException("Queue is not active.", "DOMAIN_RULE_VIOLATION_QUEUE_INACTIVE");
            }
            if (queue.TenantId != entityRequest.TenantId)
            {
                throw new DomainConflictException("Queue tenant mismatch.", "DOMAIN_CONFLICT_QUEUE_TENANT_MISMATCH");
            }

            sla = await slaRepository.GetByIdAsync(entityRequest.SlaId, cancellationToken)
                ?? throw new DomainValidationException("SLA not found.", "DOMAIN_VALIDATION_SLA_NOT_FOUND");
            if (sla.TenantId != entityRequest.TenantId)
            {
                throw new DomainConflictException("SLA tenant mismatch.", "DOMAIN_CONFLICT_SLA_TENANT_MISMATCH");
            }
        }

        var entity = mapper.Map<global::EBOS.CRM.Domain.Entities.CRM.Case>(entityRequest);
        if (!string.IsNullOrWhiteSpace(entity.Status))
        {
            var targetStatus = entity.Status;
            entity.Status = string.Empty;
            entity.SetStatus(targetStatus);
        }
        entity.SetPriority(entityRequest.Priority);

        var dueAt = entityRequest.DueAt ?? sla.CalculateDueAt(DateTime.UtcNow);
        entity.UpdateDueAt(dueAt);

        await repository.BeginTransactionAsync(cancellationToken);

        try
        {
            await repository.AddAsync(entity, cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);

            var auditRequest = new AuditInsertRequest(
                UserId: currentUser.UserId,
                TimeStamp: DateTimeOffset.UtcNow,
                Action: AuditActions.Add,
                Entity: nameof(Domain.Entities.CRM.Case),
                RegisterId: entity.Id,
                OldValues: null,
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


