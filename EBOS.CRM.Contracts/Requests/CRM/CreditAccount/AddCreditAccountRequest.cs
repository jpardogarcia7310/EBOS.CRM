namespace EBOS.CRM.Contracts.Requests.CRM.CreditAccount;

public record AddCreditAccountRequest(
    long TenantId,
    decimal MaxAmount,
    decimal UsedAmount,
    long CustomerId
);
