namespace EBOS.CRM.Contracts.Responses.CRM;

public record CreditTransactionResponse(
    long Id,
    long TenantId,
    DateTime Date,
    decimal Amount,
    string Type,
    string? ExternalReference,
    string? Comments,
    long CreditAccountId,
    bool Active
);
