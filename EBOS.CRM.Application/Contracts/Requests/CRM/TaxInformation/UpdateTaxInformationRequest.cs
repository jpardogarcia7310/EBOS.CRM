namespace EBOS.CRM.Application.Contracts.Requests.CRM.TaxInformation;

public record UpdateTaxInformationRequest(
    string TaxName,
    string TaxIdentificationNumber,
    long CustomerId
);
