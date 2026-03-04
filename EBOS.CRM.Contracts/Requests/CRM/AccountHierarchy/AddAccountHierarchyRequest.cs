namespace EBOS.CRM.Contracts.Requests.CRM.AccountHierarchy;

public record AddAccountHierarchyRequest(
    long TenantId,
    long ParentCorporateCustomerId,
    long ChildCorporateCustomerId,
    string RelationType,
    DateTime ValidFrom
);
