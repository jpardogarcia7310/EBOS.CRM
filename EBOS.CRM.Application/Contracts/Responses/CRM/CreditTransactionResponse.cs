namespace EBOS.CRM.Application.Contracts.Responses.CRM;

public record CreditTransactionResponse(
    long Id,
    DateTime Date,
    decimal Amount,
    string Type,
    string? ExternalReference,
    string? Comments,
    long CreditAccountId,
    bool Active
);
