namespace EBOS.CRM.Application.Contracts.Requests.CRM.Opportunity;

public sealed record PatchOpportunityStageRequest(
    long TenantId,
    long StageId,
    decimal? Probability
);
