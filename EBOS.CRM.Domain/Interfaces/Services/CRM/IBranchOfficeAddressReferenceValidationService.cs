namespace EBOS.CRM.Domain.Interfaces.Services.CRM;

public interface IBranchOfficeAddressReferenceValidationService
{
    Task EnsureDependenciesAvailableAsync(
        long tenantId,
        long branchOfficeId,
        long addressId,
        CancellationToken cancellationToken = default);
}
