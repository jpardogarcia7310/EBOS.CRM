namespace EBOS.CRM.Application.Contracts.Requests.CRM.Forecast;

public sealed record GetForecastRequest(
    DateTime? From,
    DateTime? To,
    long? OwnerUserId,
    long? StageId
);
