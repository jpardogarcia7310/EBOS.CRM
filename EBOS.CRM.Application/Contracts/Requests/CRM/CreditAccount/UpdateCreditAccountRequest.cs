namespace EBOS.CRM.Application.Contracts.Requests.CRM.CreditAccount;

public sealed record UpdateCreditAccountRequest(
    long Id,
    decimal MaxAmount,
    decimal UsedAmount,
    long CustomerId);
