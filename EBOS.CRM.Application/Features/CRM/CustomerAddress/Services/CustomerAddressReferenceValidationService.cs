using EBOS.CRM.Domain.Exceptions;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services.CRM;

namespace EBOS.CRM.Application.Features.CRM.CustomerAddress.Services;

public sealed class CustomerAddressReferenceValidationService(
    ICustomerRepository customerRepository,
    IAddressRepository addressRepository) : ICustomerAddressReferenceValidationService
{
    public async Task EnsureDependenciesAvailableAsync(
        long tenantId,
        long customerId,
        long addressId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var customer = await customerRepository.GetByIdAsync(customerId, cancellationToken)
                ?? throw new DomainValidationException(
                    "Customer not found for customer address.",
                    "DOMAIN_VALIDATION_CUSTOMER_ADDRESS_CUSTOMER_NOT_FOUND");
            if (customer.TenantId != tenantId)
            {
                throw new DomainConflictException(
                    "Customer address customer tenant mismatch.",
                    "DOMAIN_CONFLICT_CUSTOMER_ADDRESS_CUSTOMER_TENANT_MISMATCH");
            }
            if (customer.Erased)
            {
                throw new DomainRuleViolationException(
                    "Customer is disabled for customer address.",
                    "DOMAIN_RULE_CUSTOMER_ADDRESS_CUSTOMER_DISABLED");
            }

            var address = await addressRepository.GetByIdAsync(addressId, cancellationToken)
                ?? throw new DomainValidationException(
                    "Address not found for customer address.",
                    "DOMAIN_VALIDATION_CUSTOMER_ADDRESS_ADDRESS_NOT_FOUND");
            if (address.TenantId != tenantId)
            {
                throw new DomainConflictException(
                    "Customer address address tenant mismatch.",
                    "DOMAIN_CONFLICT_CUSTOMER_ADDRESS_ADDRESS_TENANT_MISMATCH");
            }
            if (address.Erased)
            {
                throw new DomainRuleViolationException(
                    "Address is disabled for customer address.",
                    "DOMAIN_RULE_CUSTOMER_ADDRESS_ADDRESS_DISABLED");
            }
        }
        catch (Exception ex) when (
            ex is not DomainException &&
            DomainTransientFailureClassifier.TryClassify(ex, nameof(EnsureDependenciesAvailableAsync), out _))
        {
            throw new TransientDomainFailureException(
                "Transient failure while resolving customer address dependencies.",
                "DOMAIN_TRANSIENT_CUSTOMER_ADDRESS_REFERENCE_RESOLUTION",
                ex);
        }
    }
}
