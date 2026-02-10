namespace EBOS.CRM.Application.Contracts.Requests.CRM.Customer;

public sealed record UpdateCustomerRequest(
    long Id,
    long TenantId,
    string Code,
    string Email,
    string Phone,
    long StatusId
);
