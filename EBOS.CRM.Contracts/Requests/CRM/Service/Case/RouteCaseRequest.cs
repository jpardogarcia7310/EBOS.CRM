namespace EBOS.CRM.Contracts.Requests.CRM.Service.Case;

public sealed record RouteCaseRequest(
    long TenantId,
    bool Force = false
);
