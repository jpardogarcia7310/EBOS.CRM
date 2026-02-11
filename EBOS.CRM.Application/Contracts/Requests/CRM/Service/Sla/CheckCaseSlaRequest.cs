namespace EBOS.CRM.Application.Contracts.Requests.CRM.Service.Sla;

public sealed record CheckCaseSlaRequest(
    long TenantId,
    long CaseId,
    DateTime Now
);
