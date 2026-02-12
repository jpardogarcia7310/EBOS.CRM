namespace EBOS.CRM.Contracts.Responses.EBOS;

public record TenantUsageMetricResponse(
    long Id,
    long TenantId,
    string Metric,
    decimal Value,
    string? Unit,
    DateTime PeriodStart,
    DateTime PeriodEnd,
    string? Source,
    bool Active
);
