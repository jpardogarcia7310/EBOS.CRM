namespace EBOS.CRM.Application.Contracts.Requests.CRM.Opportunity;

public sealed record WinOpportunityRequest(
    long TenantId,
    long StageId,
    string? CloseReason
);
