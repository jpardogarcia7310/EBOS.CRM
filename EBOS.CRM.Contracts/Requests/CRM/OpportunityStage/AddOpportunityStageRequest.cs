namespace EBOS.CRM.Contracts.Requests.CRM.OpportunityStage;

public record AddOpportunityStageRequest(
    long TenantId,
    string Name,
    int Order,
    decimal DefaultProbability,
    bool IsClosed,
    bool IsWon
);
