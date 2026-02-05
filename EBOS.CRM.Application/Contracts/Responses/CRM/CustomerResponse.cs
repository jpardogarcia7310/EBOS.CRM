namespace EBOS.CRM.Application.Contracts.Responses.CRM;

public record CustomerResponse(
    long Id,
    long TenantId,
    string Code,
    string Email,
    string Phone,
    long StatusId,
    bool Active
);
