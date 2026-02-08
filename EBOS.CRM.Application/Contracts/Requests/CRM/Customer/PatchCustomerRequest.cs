namespace EBOS.CRM.Application.Contracts.Requests.CRM.Customer;

public sealed record PatchCustomerRequest(
    long TenantId,
    string? Code,
    string? Email,
    string? Phone,
    long? StatusId);
