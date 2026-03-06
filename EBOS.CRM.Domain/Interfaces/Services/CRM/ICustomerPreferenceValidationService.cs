namespace EBOS.CRM.Domain.Interfaces.Services.CRM;

public interface ICustomerPreferenceValidationService
{
    Task EnsureCustomerAndChannelAvailableAsync(long tenantId, long customerId, long channelId, CancellationToken cancellationToken = default);
}
