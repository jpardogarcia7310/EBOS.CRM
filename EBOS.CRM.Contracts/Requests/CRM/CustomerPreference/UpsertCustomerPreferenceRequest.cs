namespace EBOS.CRM.Contracts.Requests.CRM.CustomerPreference;

public record UpsertCustomerPreferenceRequest(
    long TenantId,
    long CustomerId,
    long ChannelId,
    bool Preferred
);
