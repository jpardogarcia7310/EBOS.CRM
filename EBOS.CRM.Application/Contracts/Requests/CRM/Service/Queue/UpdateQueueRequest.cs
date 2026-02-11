namespace EBOS.CRM.Application.Contracts.Requests.CRM.Service.Queue;

public sealed record UpdateQueueRequest(
    long Id,
    long TenantId,
    string Name,
    string Code,
    bool IsActive,
    long? DefaultOwnerUserId
);
