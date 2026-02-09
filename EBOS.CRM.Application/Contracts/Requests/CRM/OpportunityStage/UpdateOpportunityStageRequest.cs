namespace EBOS.CRM.Application.Contracts.Requests.CRM.OpportunityStage;

public sealed record UpdateOpportunityStageRequest(
    long Id,
    long TenantId,
    string Name,
    int Order,
    decimal DefaultProbability,
    bool IsClosed,
    bool IsWon
);
