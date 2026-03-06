using EBOS.CRM.Domain.Exceptions;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services.CRM;

namespace EBOS.CRM.Application.Features.CRM.CustomerConsent.Services;

public sealed class CustomerConsentValidationService(ICustomerRepository customerRepository)
    : ICustomerConsentValidationService
{
    public async Task EnsureCustomerAvailableAsync(long tenantId, long customerId, CancellationToken cancellationToken = default)
    {
        try
        {
            var customer = await customerRepository.GetByIdAsync(customerId, cancellationToken)
                ?? throw new DomainValidationException("Customer not found.", "DOMAIN_VALIDATION_CUSTOMER_NOT_FOUND");
            if (customer.TenantId != tenantId)
            {
                throw new DomainConflictException("Customer tenant mismatch.", "DOMAIN_CONFLICT_CUSTOMER_TENANT_MISMATCH");
            }
            if (customer.Erased)
            {
                throw new DomainRuleViolationException("Customer is disabled.", "DOMAIN_RULE_CUSTOMER_DISABLED");
            }
        }
        catch (Exception ex) when (
            ex is not DomainException &&
            DomainTransientFailureClassifier.TryClassify(ex, nameof(EnsureCustomerAvailableAsync), out _))
        {
            throw new TransientDomainFailureException(
                "Transient failure while resolving customer consent dependencies.",
                "DOMAIN_TRANSIENT_CUSTOMER_CONSENT_REFERENCE_RESOLUTION",
                ex);
        }
    }
}
