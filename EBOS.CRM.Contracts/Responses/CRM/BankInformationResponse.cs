namespace EBOS.CRM.Contracts.Responses.CRM;

public record BankInformationResponse(
    long Id,
    long TenantId,
    string Iban,
    string? Bic,
    string? BankName,
    long CustomerId,
    bool Active
);
