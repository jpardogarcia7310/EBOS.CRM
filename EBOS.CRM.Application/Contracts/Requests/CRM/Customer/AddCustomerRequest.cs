namespace EBOS.CRM.Application.Contracts.Requests.CRM.Customer;

public record AddCustomerRequest(
    long TenantId,
    string Code,
    string Email,
    string Phone,
    long StatusId
);
