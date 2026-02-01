namespace EBOS.CRM.Application.Contracts.Responses.CRM;

public record CustomerResponse(
    long Id,
    string Code,
    string Email,
    string Phone,
    DateTime CreatedAt,
    long StatusId,
    bool Active
);
