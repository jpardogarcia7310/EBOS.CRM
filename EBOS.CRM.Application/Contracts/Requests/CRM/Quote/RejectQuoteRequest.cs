namespace EBOS.CRM.Application.Contracts.Requests.CRM.Quote;

public sealed record RejectQuoteRequest(
    long TenantId,
    string? Notes,
    string? Status
);
