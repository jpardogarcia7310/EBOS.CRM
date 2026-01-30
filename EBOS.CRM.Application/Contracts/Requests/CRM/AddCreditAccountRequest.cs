namespace EBOS.CRM.Application.Contracts.Requests.CRM;

public sealed record AddCreditAccountRequest(
    decimal MaxAmount,
    decimal UsedAmount,
    long CustomerId);
