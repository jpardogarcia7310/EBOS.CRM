namespace EBOS.CRM.Contracts.Requests.CRM.CustomerConsent;

public record AddCustomerConsentRequest(
    long TenantId,
    long CustomerId,
    string ConsentType,
    bool Granted,
    DateTime GrantedAt,
    string Source,
    DateTime? ExpiresAt
);
