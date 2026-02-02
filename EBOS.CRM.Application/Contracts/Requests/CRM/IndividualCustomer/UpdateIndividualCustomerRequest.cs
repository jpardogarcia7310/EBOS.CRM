namespace EBOS.CRM.Application.Contracts.Requests.CRM.IndividualCustomer;

public record UpdateIndividualCustomerRequest(
    string Code,
    string Email,
    string Phone,
    DateTime CreatedAt,
    long StatusId,
    string FirstName,
    string LastName,
    DateTime BirthDate,
    string? IdentificationNumber,
    long IdentificationTypeId
);
