namespace EBOS.CRM.Application.Contracts.Responses.CRM;

public record TenantQuotaResponse(
    long Id,
    long TenantId,
    string Metric,
    decimal Limit,
    string? Unit,
    DateTime EffectiveFrom,
    DateTime? EffectiveTo,
    bool Active
);
