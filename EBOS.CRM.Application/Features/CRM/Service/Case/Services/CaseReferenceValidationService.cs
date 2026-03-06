using EBOS.CRM.Domain.Exceptions;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services.CRM;

namespace EBOS.CRM.Application.Features.CRM.Service.Case.Services;

public sealed class CaseReferenceValidationService(
    IQueueRepository queueRepository,
    ISlaRepository slaRepository,
    ICaseRepository caseRepository) : ICaseReferenceValidationService
{
    public async Task<global::EBOS.CRM.Domain.Entities.CRM.Queue> EnsureQueueAvailableAsync(
        long tenantId,
        long queueId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var queue = await queueRepository.GetByIdAsync(queueId, cancellationToken)
                ?? throw new DomainValidationException("Queue not found.", "DOMAIN_VALIDATION_QUEUE_NOT_FOUND");
            if (!queue.IsActive)
            {
                throw new DomainRuleViolationException("Queue is not active.", "DOMAIN_RULE_VIOLATION_QUEUE_INACTIVE");
            }
            if (queue.TenantId != tenantId)
            {
                throw new DomainConflictException("Queue tenant mismatch.", "DOMAIN_CONFLICT_QUEUE_TENANT_MISMATCH");
            }

            return queue;
        }
        catch (Exception ex) when (
            ex is not DomainException &&
            DomainTransientFailureClassifier.TryClassify(ex, nameof(EnsureQueueAvailableAsync), out _))
        {
            throw new TransientDomainFailureException(
                "Transient failure while resolving case queue dependency.",
                "DOMAIN_TRANSIENT_CASE_QUEUE_REFERENCE_RESOLUTION",
                ex);
        }
    }

    public async Task<global::EBOS.CRM.Domain.Entities.CRM.Sla> EnsureSlaAvailableAsync(
        long tenantId,
        long slaId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var sla = await slaRepository.GetByIdAsync(slaId, cancellationToken)
                ?? throw new DomainValidationException("SLA not found.", "DOMAIN_VALIDATION_SLA_NOT_FOUND");
            if (sla.TenantId != tenantId)
            {
                throw new DomainConflictException("SLA tenant mismatch.", "DOMAIN_CONFLICT_SLA_TENANT_MISMATCH");
            }

            return sla;
        }
        catch (Exception ex) when (
            ex is not DomainException &&
            DomainTransientFailureClassifier.TryClassify(ex, nameof(EnsureSlaAvailableAsync), out _))
        {
            throw new TransientDomainFailureException(
                "Transient failure while resolving case SLA dependency.",
                "DOMAIN_TRANSIENT_CASE_SLA_REFERENCE_RESOLUTION",
                ex);
        }
    }

    public async Task<global::EBOS.CRM.Domain.Entities.CRM.Case> EnsureCaseAvailableForActivityAsync(
        long tenantId,
        long caseId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var caseEntity = await caseRepository.GetByIdAsync(caseId, cancellationToken)
                ?? throw new DomainValidationException("Case not found.", "DOMAIN_VALIDATION_CASE_NOT_FOUND");
            if (caseEntity.TenantId != tenantId)
            {
                throw new DomainConflictException("Case tenant mismatch.", "DOMAIN_CONFLICT_CASE_TENANT_MISMATCH");
            }
            if (caseEntity.ClosedAt.HasValue)
            {
                throw new DomainRuleViolationException("Cannot add activities to a closed case.", "DOMAIN_RULE_VIOLATION_CASE_CLOSED_ACTIVITY_ADD");
            }

            return caseEntity;
        }
        catch (Exception ex) when (
            ex is not DomainException &&
            DomainTransientFailureClassifier.TryClassify(ex, nameof(EnsureCaseAvailableForActivityAsync), out _))
        {
            throw new TransientDomainFailureException(
                "Transient failure while resolving case activity dependency.",
                "DOMAIN_TRANSIENT_CASE_ACTIVITY_REFERENCE_RESOLUTION",
                ex);
        }
    }
}
