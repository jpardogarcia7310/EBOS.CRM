namespace EBOS.CRM.Application.Contracts.Requests.CRM;

public sealed record PatchBranchOfficeRequest(
    string? Name,
    string? PhoneNumber,
    long? CorporateCustomerId);
