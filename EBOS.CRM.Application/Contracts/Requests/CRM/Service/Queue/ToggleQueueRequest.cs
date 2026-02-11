namespace EBOS.CRM.Application.Contracts.Requests.CRM.Service.Queue;

public sealed record ToggleQueueRequest(
    long TenantId,
    bool IsActive
);
