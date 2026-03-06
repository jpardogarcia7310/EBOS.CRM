using EBOS.CRM.Domain.Entities.CRM;

namespace EBOS.CRM.Domain.Interfaces.Services.CRM;

public interface IOpportunityStageValidationService
{
    Task<OpportunityStage> EnsureStageAvailableAsync(long tenantId, long stageId, CancellationToken cancellationToken = default);
}
