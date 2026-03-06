namespace EBOS.CRM.Domain.Interfaces.Services.CRM;

public interface IBankInformationReferenceValidationService
{
    Task EnsureCustomerAvailableAsync(
        long tenantId,
        long customerId,
        CancellationToken cancellationToken = default);
}
