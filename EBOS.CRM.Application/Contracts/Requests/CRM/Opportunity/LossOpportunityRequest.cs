namespace EBOS.CRM.Application.Contracts.Requests.CRM.Opportunity;

public sealed record LossOpportunityRequest(
    long TenantId,
    long StageId,
    string? CloseReason
);
