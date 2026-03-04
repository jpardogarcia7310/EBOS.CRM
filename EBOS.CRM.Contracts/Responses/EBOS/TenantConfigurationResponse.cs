namespace EBOS.CRM.Contracts.Responses.EBOS;

public record TenantConfigurationResponse(
    long Id,
    long TenantId,
    string Key,
    string ValueJson,
    bool Active
);
