namespace EBOS.CRM.Contracts.Requests.CRM.Service.Case;

public sealed record AssignCaseSlaRequest(
    long TenantId,
    long SlaId
);
