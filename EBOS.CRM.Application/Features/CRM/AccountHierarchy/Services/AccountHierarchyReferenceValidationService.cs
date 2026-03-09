using EBOS.CRM.Domain.Exceptions;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services.CRM;

namespace EBOS.CRM.Application.Features.CRM.AccountHierarchy.Services;

public sealed class AccountHierarchyReferenceValidationService(ICorporateCustomerRepository corporateCustomerRepository) : IAccountHierarchyReferenceValidationService
{
    public async Task<global::EBOS.CRM.Domain.Entities.CRM.CorporateCustomer> EnsureCorporateCustomerAvailableAsync(long tenantId, long corporateCustomerId, string role, CancellationToken cancellationToken = default)
    {
        try
        {
            var code = role.Equals("Parent", StringComparison.OrdinalIgnoreCase)
                ? "DOMAIN_VALIDATION_PARENT_CORPORATE_CUSTOMER_NOT_FOUND"
                : "DOMAIN_VALIDATION_CHILD_CORPORATE_CUSTOMER_NOT_FOUND";
            var mismatch = role.Equals("Parent", StringComparison.OrdinalIgnoreCase)
                ? "DOMAIN_CONFLICT_PARENT_CORPORATE_CUSTOMER_TENANT_MISMATCH"
                : "DOMAIN_CONFLICT_CHILD_CORPORATE_CUSTOMER_TENANT_MISMATCH";

            var corporateCustomer = await corporateCustomerRepository.GetByIdAsync(corporateCustomerId, cancellationToken)
                ?? throw new DomainValidationException($"{role} corporate customer not found.", code);
            if (corporateCustomer.TenantId != tenantId)
            {
                throw new DomainConflictException($"{role} corporate customer tenant mismatch.", mismatch);
            }

            return corporateCustomer;
        }
        catch (Exception ex) when (
            ex is not DomainException &&
            DomainTransientFailureClassifier.TryClassify(ex, nameof(EnsureCorporateCustomerAvailableAsync), out _))
        {
            throw new TransientDomainFailureException(
                "Transient failure while validating account hierarchy corporate customer reference.",
                "DOMAIN_TRANSIENT_ACCOUNT_HIERARCHY_CORPORATE_CUSTOMER_LOOKUP",
                ex);
        }
    }
}
