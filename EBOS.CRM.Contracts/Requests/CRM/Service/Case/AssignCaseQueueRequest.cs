namespace EBOS.CRM.Contracts.Requests.CRM.Service.Case;

public sealed record AssignCaseQueueRequest(
    long TenantId,
    long QueueId
);
