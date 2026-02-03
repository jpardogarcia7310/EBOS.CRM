namespace EBOS.CRM.Application.Contracts.Requests.CRM.BranchOffice;

public sealed record PatchBranchOfficeRequest(
    string? Name,
    string? PhoneNumber,
    long? CorporateCustomerId);
