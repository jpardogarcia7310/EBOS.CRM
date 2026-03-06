using EBOS.CRM.Domain.Exceptions;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services.CRM;

namespace EBOS.CRM.Application.Features.CRM.Quote.Services;

public sealed class QuoteOpportunityValidationService(IOpportunityRepository opportunityRepository)
    : IQuoteOpportunityValidationService
{
    public async Task EnsureOpportunityAvailableAsync(
        long tenantId,
        long opportunityId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var opportunity = await opportunityRepository.GetByIdAsync(opportunityId, cancellationToken)
                ?? throw new DomainValidationException(
                    "Opportunity not found for quote.",
                    "DOMAIN_VALIDATION_QUOTE_OPPORTUNITY_NOT_FOUND");

            if (opportunity.TenantId != tenantId)
            {
                throw new DomainConflictException(
                    "Quote opportunity tenant mismatch.",
                    "DOMAIN_CONFLICT_QUOTE_OPPORTUNITY_TENANT_MISMATCH");
            }

            if (opportunity.Erased)
            {
                throw new DomainRuleViolationException(
                    "Quote cannot target a deleted opportunity.",
                    "DOMAIN_RULE_QUOTE_OPPORTUNITY_DISABLED");
            }
        }
        catch (Exception ex) when (
            ex is not DomainException &&
            DomainTransientFailureClassifier.TryClassify(ex, nameof(EnsureOpportunityAvailableAsync), out _))
        {
            throw new TransientDomainFailureException(
                "Transient failure while resolving quote opportunity.",
                "DOMAIN_TRANSIENT_QUOTE_OPPORTUNITY_RESOLUTION",
                ex);
        }
    }
}
