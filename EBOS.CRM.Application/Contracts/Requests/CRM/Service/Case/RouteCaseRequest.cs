namespace EBOS.CRM.Application.Contracts.Requests.CRM.Service.Case;

public sealed record RouteCaseRequest(
    bool Force = false
);
