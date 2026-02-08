namespace EBOS.CRM.Application.Contracts.Requests.CRM.BankInformation;

public record AddBankInformationRequest(
    long TenantId,
    string Iban,
    string? Bic,
    string? BankName,
    long CustomerId
);
