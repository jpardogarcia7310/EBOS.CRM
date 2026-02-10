namespace EBOS.CRM.Application.Contracts.Requests.CRM.Quote;

public record AddQuoteRequest(
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
