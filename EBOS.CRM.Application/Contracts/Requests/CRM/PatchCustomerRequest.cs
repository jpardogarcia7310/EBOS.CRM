namespace EBOS.CRM.Application.Contracts.Requests.CRM;

public sealed record PatchCustomerRequest(
    string? Code,
    string? Email,
    string? Phone,
    long? StatusId);
