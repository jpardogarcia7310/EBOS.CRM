namespace EBOS.CRM.Application.Contracts.Requests.CRM.Service.Sla;

public sealed record AddSlaRequest(
    long TenantId,
    string Name,
    int TargetMinutes,
    int? WarningMinutes,
    DateTime? ActiveFrom,
    DateTime? ActiveTo,
    bool IsActive
);
