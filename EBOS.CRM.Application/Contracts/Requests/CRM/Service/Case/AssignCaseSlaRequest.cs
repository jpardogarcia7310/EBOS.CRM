namespace EBOS.CRM.Application.Contracts.Requests.CRM.Service.Case;

public sealed record AssignCaseSlaRequest(
    long TenantId,
    long SlaId
);
