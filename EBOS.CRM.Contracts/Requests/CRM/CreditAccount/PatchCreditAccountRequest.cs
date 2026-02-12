namespace EBOS.CRM.Contracts.Requests.CRM.CreditAccount;

public sealed record PatchCreditAccountRequest(
    long TenantId,
    decimal? MaxAmount,
    decimal? UsedAmount,
    long? CustomerId
);
