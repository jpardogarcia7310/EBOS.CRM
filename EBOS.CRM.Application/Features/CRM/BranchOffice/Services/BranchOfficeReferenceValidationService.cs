using EBOS.CRM.Domain.Exceptions;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services.CRM;

namespace EBOS.CRM.Application.Features.CRM.BranchOffice.Services;

public sealed class BranchOfficeReferenceValidationService(ICorporateCustomerRepository corporateCustomerRepository)
    : IBranchOfficeReferenceValidationService
{
    public async Task EnsureCorporateCustomerAvailableAsync(
        long tenantId,
        long corporateCustomerId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var corporateCustomer = await corporateCustomerRepository.GetByIdAsync(corporateCustomerId, cancellationToken)
                ?? throw new DomainValidationException(
                    "Corporate customer not found for branch office.",
                    "DOMAIN_VALIDATION_BRANCH_OFFICE_CORPORATE_CUSTOMER_NOT_FOUND");
            if (corporateCustomer.TenantId != tenantId)
            {
                throw new DomainConflictException(
                    "Branch office corporate customer tenant mismatch.",
                    "DOMAIN_CONFLICT_BRANCH_OFFICE_CORPORATE_CUSTOMER_TENANT_MISMATCH");
            }
            if (corporateCustomer.Erased)
            {
                throw new DomainRuleViolationException(
                    "Corporate customer is disabled for branch office.",
                    "DOMAIN_RULE_BRANCH_OFFICE_CORPORATE_CUSTOMER_DISABLED");
            }
        }
        catch (Exception ex) when (
            ex is not DomainException &&
            DomainTransientFailureClassifier.TryClassify(ex, nameof(EnsureCorporateCustomerAvailableAsync), out _))
        {
            throw new TransientDomainFailureException(
                "Transient failure while resolving branch office references.",
                "DOMAIN_TRANSIENT_BRANCH_OFFICE_REFERENCE_RESOLUTION",
                ex);
        }
    }
}
