namespace EBOS.CRM.Domain.Interfaces.Services.CRM;

public interface IBranchOfficeReferenceValidationService
{
    Task EnsureCorporateCustomerAvailableAsync(
        long tenantId,
        long corporateCustomerId,
        CancellationToken cancellationToken = default);
}
