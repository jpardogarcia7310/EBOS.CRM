namespace EBOS.CRM.Application.Contracts.Requests.CRM.CreditTransaction;

public record AddCreditTransactionRequest(
    DateTime Date,
    decimal Amount,
    string Type,
    string ExternalReference,
    string Comments,
    long CreditAccountId
);
