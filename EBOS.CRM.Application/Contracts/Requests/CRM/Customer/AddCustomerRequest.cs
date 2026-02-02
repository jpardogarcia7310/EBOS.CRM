namespace EBOS.CRM.Application.Contracts.Requests.CRM.Customer;

public record AddCustomerRequest(
    string Code,
    string Email,
    string Phone,
    DateTime CreatedAt,
    long StatusId
);
