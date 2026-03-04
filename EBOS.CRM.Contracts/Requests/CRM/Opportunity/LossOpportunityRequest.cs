namespace EBOS.CRM.Contracts.Requests.CRM.Opportunity;

public sealed record LossOpportunityRequest(
    long TenantId,
    long StageId,
    string? CloseReason
);
