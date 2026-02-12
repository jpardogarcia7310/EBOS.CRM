namespace EBOS.CRM.Contracts.Responses.CRM;

public record CustomerConsentResponse(
    long Id,
    long TenantId,
    long CustomerId,
    string ConsentType,
    bool Granted,
    DateTime GrantedAt,
    string Source,
    DateTime? ExpiresAt,
    DateTime? RevokedAt,
    bool Active
);
