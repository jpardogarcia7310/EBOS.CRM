namespace EBOS.CRM.Application.Contracts.Requests.CRM.TaxInformation;

public sealed record PatchTaxInformationRequest(
    string? TaxName,
    string? TaxIdentificationNumber,
    long? CustomerId
);
