using EBOS.CRM.Domain.Exceptions;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services.CRM;

namespace EBOS.CRM.Application.Features.CRM.BankInformation.Services;

public sealed class BankInformationReferenceValidationService(ICustomerRepository customerRepository)
    : IBankInformationReferenceValidationService
{
    public async Task EnsureCustomerAvailableAsync(
        long tenantId,
        long customerId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var customer = await customerRepository.GetByIdAsync(customerId, cancellationToken)
                ?? throw new DomainValidationException(
                    "Customer not found for bank information.",
                    "DOMAIN_VALIDATION_BANK_INFORMATION_CUSTOMER_NOT_FOUND");
            if (customer.TenantId != tenantId)
            {
                throw new DomainConflictException(
                    "Bank information customer tenant mismatch.",
                    "DOMAIN_CONFLICT_BANK_INFORMATION_CUSTOMER_TENANT_MISMATCH");
            }
            if (customer.Erased)
            {
                throw new DomainRuleViolationException(
                    "Customer is disabled for bank information.",
                    "DOMAIN_RULE_BANK_INFORMATION_CUSTOMER_DISABLED");
            }
        }
        catch (Exception ex) when (
            ex is not DomainException &&
            DomainTransientFailureClassifier.TryClassify(ex, nameof(EnsureCustomerAvailableAsync), out _))
        {
            throw new TransientDomainFailureException(
                "Transient failure while resolving bank information customer.",
                "DOMAIN_TRANSIENT_BANK_INFORMATION_REFERENCE_RESOLUTION",
                ex);
        }
    }
}
