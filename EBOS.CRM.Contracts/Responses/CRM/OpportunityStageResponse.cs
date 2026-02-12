namespace EBOS.CRM.Contracts.Responses.CRM;

public record OpportunityStageResponse(
    long Id,
    long TenantId,
    string Name,
    int Order,
    decimal DefaultProbability,
    bool IsClosed,
    bool IsWon,
    bool Active
);
