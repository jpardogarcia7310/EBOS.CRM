namespace EBOS.CRM.Application.Contracts.Requests.CRM.CreditAccount;

public record UpdateCreditAccountRequest(
    decimal MaxAmount,
    decimal UsedAmount,
    long CustomerId
);
