namespace EBOS.CRM.Contracts.Requests.CRM.Quote;

public sealed record ApproveQuoteRequest(
    long TenantId,
    string? Notes,
    string? Status
);
