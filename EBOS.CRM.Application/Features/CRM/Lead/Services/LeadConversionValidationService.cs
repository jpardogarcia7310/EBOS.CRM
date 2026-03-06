using EBOS.CRM.Domain.Exceptions;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services.CRM;

namespace EBOS.CRM.Application.Features.CRM.Lead.Services;

public sealed class LeadConversionValidationService(
    ICustomerRepository customerRepository,
    IOpportunityStageRepository stageRepository) : ILeadConversionValidationService
{
    public async Task EnsureDependenciesAvailableAsync(
        long tenantId,
        long customerId,
        long stageId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var customer = await customerRepository.GetByIdAsync(customerId, cancellationToken)
                ?? throw new DomainValidationException(
                    "Customer not found for lead conversion.",
                    "DOMAIN_VALIDATION_LEAD_CONVERSION_CUSTOMER_NOT_FOUND");
            if (customer.TenantId != tenantId)
            {
                throw new DomainConflictException(
                    "Lead conversion customer tenant mismatch.",
                    "DOMAIN_CONFLICT_LEAD_CONVERSION_CUSTOMER_TENANT_MISMATCH");
            }

            var stage = await stageRepository.GetByIdAsync(stageId, cancellationToken)
                ?? throw new DomainValidationException(
                    "Opportunity stage not found for lead conversion.",
                    "DOMAIN_VALIDATION_LEAD_CONVERSION_STAGE_NOT_FOUND");
            if (stage.TenantId != tenantId)
            {
                throw new DomainConflictException(
                    "Lead conversion stage tenant mismatch.",
                    "DOMAIN_CONFLICT_LEAD_CONVERSION_STAGE_TENANT_MISMATCH");
            }

            if (customer.Erased || stage.Erased)
            {
                throw new DomainRuleViolationException(
                    "Lead conversion dependencies are not active.",
                    "DOMAIN_RULE_LEAD_CONVERSION_DEPENDENCY_INACTIVE");
            }
        }
        catch (Exception ex) when (
            ex is not DomainException &&
            DomainTransientFailureClassifier.TryClassify(ex, nameof(EnsureDependenciesAvailableAsync), out _))
        {
            throw new TransientDomainFailureException(
                "Transient failure while resolving lead conversion dependencies.",
                "DOMAIN_TRANSIENT_LEAD_CONVERSION_DEPENDENCY_RESOLUTION",
                ex);
        }
    }
}
