namespace EBOS.CRM.Contracts.Requests.CRM.Service.Sla;

public sealed record UpdateSlaRequest(
    long Id,
    long TenantId,
    string Name,
    int TargetMinutes,
    int? WarningMinutes,
    DateTime? ActiveFrom,
    DateTime? ActiveTo,
    bool IsActive
);
