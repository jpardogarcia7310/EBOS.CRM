namespace EBOS.CRM.Contracts.Requests.CRM.AccountContactRole;

public record AddAccountContactRoleRequest(
    long TenantId,
    long AccountContactId,
    string RoleCode,
    bool IsPrimary,
    DateTime ValidFrom,
    DateTime? ValidTo
);
