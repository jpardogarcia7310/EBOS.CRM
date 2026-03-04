namespace EBOS.CRM.Contracts.Requests.CRM.Customer;

public sealed record UpdateCustomerRequest(
    long Id,
    long TenantId,
    string Code,
    string Email,
    string Phone,
    long StatusId,
    long? CountryId = null
);
