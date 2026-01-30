namespace EBOS.CRM.Application.Contracts.Responses.CRM;

public sealed record CreditAccountResponse(
    long Id,
    decimal MaxAmount,
    decimal UsedAmount,
    decimal AvailableAmount,
    long CustomerId,
    bool Active);
