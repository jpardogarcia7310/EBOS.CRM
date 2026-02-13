namespace EBOS.CRM.Contracts.Requests.CRM.AccountHierarchy;

public record EndAccountHierarchyRequest(
    long TenantId,
    DateTime ValidTo
);
