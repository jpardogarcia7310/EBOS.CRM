namespace EBOS.CRM.Contracts.Requests.CRM.Service.Sla;

public sealed record CheckSlaBatchRequest(
    long TenantId,
    DateTime Now,
    int PageNumber = 1,
    int PageSize = 100
);
