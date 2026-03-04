namespace EBOS.CRM.Contracts.Responses.CRM;

public record AccountContactRoleResponse(
    long Id,
    long TenantId,
    long AccountContactId,
    string RoleCode,
    bool IsPrimary,
    DateTime ValidFrom,
    DateTime? ValidTo,
    bool Active
);
