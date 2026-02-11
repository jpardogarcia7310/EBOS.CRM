namespace EBOS.CRM.Application.Contracts.Responses.CRM;

public record SlaResponse(
    long Id,
    long TenantId,
    string Name,
    int TargetMinutes,
    int? WarningMinutes,
    DateTime? ActiveFrom,
    DateTime? ActiveTo,
    bool IsActive,
    bool Active
);
