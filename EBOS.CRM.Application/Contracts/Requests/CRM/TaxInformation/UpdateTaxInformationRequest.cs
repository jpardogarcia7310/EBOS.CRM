namespace EBOS.CRM.Application.Contracts.Requests.CRM.TaxInformation;

public sealed record UpdateTaxInformationRequest(
    long Id,
    string TaxName,
    string TaxIdentificationNumber,
    long CustomerId
);
