namespace EBOS.CRM.Domain.Interfaces.Services.CRM;

public interface ICustomerConsentValidationService
{
    Task EnsureCustomerAvailableAsync(long tenantId, long customerId, CancellationToken cancellationToken = default);
}
