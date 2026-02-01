namespace EBOS.CRM.Application.Contracts.Requests.CRM.BankInformation;

public record UpdateBankInformationRequest(
    string Iban,
    string? Bic,
    string? BankName,
    long CustomerId
);
