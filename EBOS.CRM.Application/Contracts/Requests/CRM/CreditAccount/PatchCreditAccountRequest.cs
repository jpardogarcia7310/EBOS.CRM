namespace EBOS.CRM.Application.Contracts.Requests.CRM.CreditAccount;

public sealed record PatchCreditAccountRequest(
    decimal? MaxAmount,
    decimal? UsedAmount,
    long? CustomerId
);
