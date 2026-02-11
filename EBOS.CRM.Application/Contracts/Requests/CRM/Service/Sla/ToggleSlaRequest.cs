namespace EBOS.CRM.Application.Contracts.Requests.CRM.Service.Sla;

public sealed record ToggleSlaRequest(
    long TenantId,
    bool IsActive
);
