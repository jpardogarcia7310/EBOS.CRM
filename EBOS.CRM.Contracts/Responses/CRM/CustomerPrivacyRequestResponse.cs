namespace EBOS.CRM.Contracts.Responses.CRM;

public sealed record CustomerPrivacyRequestResponse(
    long Id,
    long TenantId,
    long CustomerId,
    string RequestType,
    string Status,
    string? Reason,
    long RequestedBy,
    DateTime RequestedAt,
    long? ProcessedBy,
    DateTime? ProcessedAt,
    string? FailureCode,
    string? FailureReason,
    string? CorrelationId);
