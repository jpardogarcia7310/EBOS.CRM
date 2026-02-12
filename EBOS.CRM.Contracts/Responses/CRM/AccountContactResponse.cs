namespace EBOS.CRM.Contracts.Responses.CRM;

public record AccountContactResponse(
    long Id,
    long TenantId,
    long CorporateCustomerId,
    long IndividualCustomerId,
    bool IsPrimary,
    DateTime StartAt,
    DateTime? EndAt,
    bool Active
);
