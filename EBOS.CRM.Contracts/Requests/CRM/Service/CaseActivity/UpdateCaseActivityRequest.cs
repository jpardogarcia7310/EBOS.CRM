namespace EBOS.CRM.Contracts.Requests.CRM.Service.CaseActivity;

public sealed record UpdateCaseActivityRequest(
    long Id,
    long TenantId,
    long CaseId,
    string Title,
    string? Description,
    string Status
);
