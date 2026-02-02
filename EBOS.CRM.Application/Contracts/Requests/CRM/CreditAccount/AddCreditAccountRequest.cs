namespace EBOS.CRM.Application.Contracts.Requests.CRM.CreditAccount;

public record AddCreditAccountRequest(
    decimal MaxAmount,
    decimal UsedAmount,
    long CustomerId
);
