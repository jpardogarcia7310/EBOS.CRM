namespace EBOS.CRM.Contracts.Requests.CRM.Forecast;

public sealed record GetForecastRequest(
    long TenantId,
    DateTime? From,
    DateTime? To,
    long? OwnerUserId,
    long? StageId
);
