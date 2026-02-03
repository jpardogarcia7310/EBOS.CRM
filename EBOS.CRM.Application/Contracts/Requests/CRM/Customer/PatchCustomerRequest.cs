namespace EBOS.CRM.Application.Contracts.Requests.CRM.Customer;

public sealed record PatchCustomerRequest(
    string? Code,
    string? Email,
    string? Phone,
    long? StatusId);
