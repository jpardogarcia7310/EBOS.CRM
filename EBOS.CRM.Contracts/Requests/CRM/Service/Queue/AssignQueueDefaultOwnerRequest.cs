namespace EBOS.CRM.Contracts.Requests.CRM.Service.Queue;

public sealed record AssignQueueDefaultOwnerRequest(
    long TenantId,
    long? DefaultOwnerUserId
);
