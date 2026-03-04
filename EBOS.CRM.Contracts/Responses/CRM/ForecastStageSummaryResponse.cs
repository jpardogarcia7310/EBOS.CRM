namespace EBOS.CRM.Contracts.Responses.CRM;

public record ForecastStageSummaryResponse(
    long StageId,
    string StageName,
    int OpportunityCount,
    decimal TotalAmount,
    decimal WeightedAmount
);
