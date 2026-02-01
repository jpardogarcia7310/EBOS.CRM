namespace EBOS.CRM.Application.Contracts.Requests.CRM.Customer;

public record UpdateCustomerRequest(
    string Code,
    string Email,
    string Phone,
    DateTime CreatedAt,
    long StatusId
);
