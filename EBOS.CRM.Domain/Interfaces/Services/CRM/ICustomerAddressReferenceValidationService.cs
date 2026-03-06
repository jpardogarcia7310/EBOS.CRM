namespace EBOS.CRM.Domain.Interfaces.Services.CRM;

public interface ICustomerAddressReferenceValidationService
{
    Task EnsureDependenciesAvailableAsync(
        long tenantId,
        long customerId,
        long addressId,
        CancellationToken cancellationToken = default);
}
