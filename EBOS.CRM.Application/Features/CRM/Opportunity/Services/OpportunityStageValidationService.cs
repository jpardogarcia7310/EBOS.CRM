using EBOS.CRM.Domain.Exceptions;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services.CRM;

namespace EBOS.CRM.Application.Features.CRM.Opportunity.Services;

public sealed class OpportunityStageValidationService(IOpportunityStageRepository stageRepository)
    : IOpportunityStageValidationService
{
    public async Task<global::EBOS.CRM.Domain.Entities.CRM.OpportunityStage> EnsureStageAvailableAsync(
        long tenantId,
        long stageId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var stage = await stageRepository.GetByIdAsync(stageId, cancellationToken)
                ?? throw new DomainValidationException(
                    "Opportunity stage not found.",
                    "DOMAIN_VALIDATION_OPPORTUNITY_STAGE_NOT_FOUND");

            if (stage.TenantId != tenantId)
            {
                throw new DomainConflictException(
                    "Opportunity stage tenant mismatch.",
                    "DOMAIN_CONFLICT_OPPORTUNITY_STAGE_TENANT_MISMATCH");
            }

            if (stage.Erased)
            {
                throw new DomainRuleViolationException(
                    "Opportunity stage is disabled and cannot be used.",
                    "DOMAIN_RULE_OPPORTUNITY_STAGE_DISABLED");
            }

            return stage;
        }
        catch (Exception ex) when (
            ex is not DomainException &&
            DomainTransientFailureClassifier.TryClassify(ex, nameof(EnsureStageAvailableAsync), out _))
        {
            throw new TransientDomainFailureException(
                "Transient failure while resolving opportunity stage.",
                "DOMAIN_TRANSIENT_OPPORTUNITY_STAGE_RESOLUTION",
                ex);
        }
    }
}
