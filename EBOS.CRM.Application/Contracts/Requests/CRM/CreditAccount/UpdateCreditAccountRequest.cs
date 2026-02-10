namespace EBOS.CRM.Application.Contracts.Requests.CRM.CreditAccount;

public sealed record UpdateCreditAccountRequest(
    long Id,
    long TenantId,
    decimal MaxAmount,
    decimal UsedAmount,
    long CustomerId
);
