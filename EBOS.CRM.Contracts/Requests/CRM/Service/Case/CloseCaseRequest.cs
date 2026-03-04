namespace EBOS.CRM.Contracts.Requests.CRM.Service.Case;

public sealed record CloseCaseRequest(
    long TenantId,
    DateTime ClosedAt
);
