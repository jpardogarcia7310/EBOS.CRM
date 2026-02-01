namespace EBOS.CRM.Application.Contracts.Requests.CRM;

public sealed record PatchTaxInformationRequest(
    string? TaxName,
    string? TaxIdentificationNumber,
    long? CustomerId);
