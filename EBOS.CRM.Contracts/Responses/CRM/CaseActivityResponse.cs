namespace EBOS.CRM.Contracts.Responses.CRM;

public record CaseActivityResponse(
    long Id,
    long TenantId,
    long CaseId,
    string Title,
    string? Description,
    string Status,
    bool Active
);
