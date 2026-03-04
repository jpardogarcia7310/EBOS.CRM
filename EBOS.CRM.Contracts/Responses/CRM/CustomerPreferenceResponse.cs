namespace EBOS.CRM.Contracts.Responses.CRM;

public record CustomerPreferenceResponse(
    long Id,
    long TenantId,
    long CustomerId,
    long ChannelId,
    bool Preferred,
    bool Active
);
