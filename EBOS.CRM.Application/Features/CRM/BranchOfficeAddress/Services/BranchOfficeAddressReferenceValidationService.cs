using EBOS.CRM.Domain.Exceptions;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services.CRM;

namespace EBOS.CRM.Application.Features.CRM.BranchOfficeAddress.Services;

public sealed class BranchOfficeAddressReferenceValidationService(
    IBranchOfficeRepository branchOfficeRepository,
    IAddressRepository addressRepository) : IBranchOfficeAddressReferenceValidationService
{
    public async Task EnsureDependenciesAvailableAsync(
        long tenantId,
        long branchOfficeId,
        long addressId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var branchOffice = await branchOfficeRepository.GetByIdAsync(branchOfficeId, cancellationToken)
                ?? throw new DomainValidationException(
                    "Branch office not found.",
                    "DOMAIN_VALIDATION_BRANCH_OFFICE_NOT_FOUND");
            if (branchOffice.TenantId != tenantId)
            {
                throw new DomainConflictException(
                    "Branch office tenant mismatch.",
                    "DOMAIN_CONFLICT_BRANCH_OFFICE_TENANT_MISMATCH");
            }
            if (branchOffice.Erased)
            {
                throw new DomainRuleViolationException(
                    "Branch office is disabled.",
                    "DOMAIN_RULE_BRANCH_OFFICE_DISABLED");
            }

            var address = await addressRepository.GetByIdAsync(addressId, cancellationToken)
                ?? throw new DomainValidationException(
                    "Address not found.",
                    "DOMAIN_VALIDATION_ADDRESS_NOT_FOUND");
            if (address.TenantId != tenantId)
            {
                throw new DomainConflictException(
                    "Address tenant mismatch.",
                    "DOMAIN_CONFLICT_ADDRESS_TENANT_MISMATCH");
            }
            if (address.Erased)
            {
                throw new DomainRuleViolationException(
                    "Address is disabled.",
                    "DOMAIN_RULE_ADDRESS_DISABLED");
            }
        }
        catch (Exception ex) when (
            ex is not DomainException &&
            DomainTransientFailureClassifier.TryClassify(ex, nameof(EnsureDependenciesAvailableAsync), out _))
        {
            throw new TransientDomainFailureException(
                "Transient failure while resolving branch office address dependencies.",
                "DOMAIN_TRANSIENT_BRANCH_OFFICE_ADDRESS_REFERENCE_RESOLUTION",
                ex);
        }
    }
}
