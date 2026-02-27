namespace EBOS.CRM.Contracts.Requests.CRM.CustomerConsent;

public record RevokeCustomerConsentRequest(
    long TenantId,
    DateTime RevokedAt
);
