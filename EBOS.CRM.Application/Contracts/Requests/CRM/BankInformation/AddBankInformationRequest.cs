namespace EBOS.CRM.Application.Contracts.Requests.CRM.BankInformation;

public record AddBankInformationRequest(
    string Iban,
    string? Bic,
    string? BankName,
    long CustomerId
);
