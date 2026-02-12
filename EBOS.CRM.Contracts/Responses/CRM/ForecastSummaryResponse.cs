namespace EBOS.CRM.Contracts.Responses.CRM;

public record ForecastSummaryResponse(
    DateTime? From,
    DateTime? To,
    long? OwnerUserId,
    long? StageId,
    IReadOnlyCollection<ForecastStageSummaryResponse> Stages,
    decimal TotalAmount,
    decimal WeightedAmount
);
