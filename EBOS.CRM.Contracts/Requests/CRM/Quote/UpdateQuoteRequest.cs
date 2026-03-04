namespace EBOS.CRM.Contracts.Requests.CRM.Quote;

public sealed record UpdateQuoteRequest(
    long Id,
    long TenantId,
    long OpportunityId,
    string Status,
    string? ReferenceNumber,
    decimal SubtotalAmount,
    decimal DiscountAmount,
    decimal TotalAmount,
    DateTime? ValidUntil,
    string? Notes
);
