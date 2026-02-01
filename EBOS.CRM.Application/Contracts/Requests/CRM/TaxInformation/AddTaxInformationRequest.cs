namespace EBOS.CRM.Application.Contracts.Requests.CRM.TaxInformation;

public record AddTaxInformationRequest(
    string TaxName,
    string TaxIdentificationNumber,
    long CustomerId
);
