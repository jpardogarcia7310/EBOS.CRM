namespace EBOS.CRM.Application.Contracts.Responses.CRM;

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
