using EBOS.CRM.Domain.Entities.CRM;

namespace EBOS.CRM.Domain.Interfaces.Services.CRM;

public interface ICaseReferenceValidationService
{
    Task<Queue> EnsureQueueAvailableAsync(long tenantId, long queueId, CancellationToken cancellationToken = default);
    Task<Sla> EnsureSlaAvailableAsync(long tenantId, long slaId, CancellationToken cancellationToken = default);
    Task<Case> EnsureCaseAvailableForActivityAsync(long tenantId, long caseId, CancellationToken cancellationToken = default);
}
