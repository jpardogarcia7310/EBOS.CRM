namespace EBOS.CRM.Application.Contracts.Requests.CRM.Service.Case;

public sealed record AssignCaseOwnerRequest(
    long TenantId,
    long OwnerUserId
);
