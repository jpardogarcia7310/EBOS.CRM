namespace EBOS.CRM.Application.Contracts.Responses.CRM;

public sealed record QuoteResponse(
    long Id,
    long TenantId,
    long OpportunityId,
    string Status,
    string? ReferenceNumber,
    decimal SubtotalAmount,
    decimal DiscountAmount,
    decimal TotalAmount,
    DateTime? ValidUntil,
    string? Notes,
    bool Active
);
