namespace EBOS.CRM.Application.Contracts.Requests.CRM;

public sealed record PatchCreditAccountRequest(
    decimal? MaxAmount,
    decimal? UsedAmount,
    long? CustomerId);
