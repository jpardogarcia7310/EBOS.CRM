namespace EBOS.CRM.Application.Contracts.Requests.CRM;

public sealed record UpdateCreditAccountRequest(
    long Id,
    decimal MaxAmount,
    decimal UsedAmount,
    long CustomerId);
