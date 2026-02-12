namespace EBOS.CRM.Contracts.Requests.CRM.Opportunity;

public sealed record CloseOpportunityRequest(
    long TenantId,
    long StageId,
    bool IsWon,
    string? CloseReason
);
