namespace EBOS.CRM.Contracts.Responses.CRM;

public record AccountHierarchyResponse(
    long Id,
    long TenantId,
    long ParentCorporateCustomerId,
    long ChildCorporateCustomerId,
    string RelationType,
    DateTime ValidFrom,
    DateTime? ValidTo,
    bool IsCurrent,
    bool Active
);
