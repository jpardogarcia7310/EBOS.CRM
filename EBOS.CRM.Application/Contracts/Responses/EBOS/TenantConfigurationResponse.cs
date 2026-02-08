namespace EBOS.CRM.Application.Contracts.Responses.CRM;

public record TenantConfigurationResponse(
    long Id,
    long TenantId,
    string Key,
    string ValueJson,
    bool Active
);
