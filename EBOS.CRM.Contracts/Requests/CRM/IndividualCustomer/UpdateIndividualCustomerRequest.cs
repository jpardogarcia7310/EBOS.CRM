namespace EBOS.CRM.Contracts.Requests.CRM.IndividualCustomer;

public record UpdateIndividualCustomerRequest(
    long TenantId,
    string Code,
    string Email,
    string Phone,
    long StatusId,
    string FirstName,
    string LastName,
    DateTime BirthDate,
    string? IdentificationNumber,
    long IdentificationTypeId
);
