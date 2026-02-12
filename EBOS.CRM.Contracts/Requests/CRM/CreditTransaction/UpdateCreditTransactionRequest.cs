namespace EBOS.CRM.Contracts.Requests.CRM.CreditTransaction;

public record UpdateCreditTransactionRequest(
    long TenantId,
    DateTime Date,
    decimal Amount,
    string Type,
    string ExternalReference,
    string Comments,
    long CreditAccountId
);
