namespace EBOS.CRM.Contracts.Requests.CRM.AccountContactRole;

public record UpdateAccountContactRoleRequest(
    long TenantId,
    long AccountContactId,
    string RoleCode,
    bool IsPrimary,
    DateTime ValidFrom,
    DateTime? ValidTo
);
