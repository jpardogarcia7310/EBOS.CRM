namespace EBOS.CRM.Application.Contracts.Requests.CRM.Service.CaseActivity;

public sealed record AddCaseActivityRequest(
    long TenantId,
    long CaseId,
    string Title,
    string? Description,
    string Status
);
