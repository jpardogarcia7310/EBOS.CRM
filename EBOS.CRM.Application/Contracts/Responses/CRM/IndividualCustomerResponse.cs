namespace EBOS.CRM.Application.Contracts.Responses.CRM;

public record IndividualCustomerResponse(
    long Id,
    long TenantId,
    string Code,
    string Email,
    string Phone,
    long StatusId,
    string FirstName,
    string LastName,
    DateTime BirthDate,
    string? IdentificationNumber,
    long IdentificationTypeId,
    bool Active
);
