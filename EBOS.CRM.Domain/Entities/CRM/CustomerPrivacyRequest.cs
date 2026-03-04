using EBOS.Core.Primitives;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;

namespace EBOS.CRM.Domain.Entities.CRM;

public class CustomerPrivacyRequest : ErasableEntity, ITenantScopedEntity
{
    public const string TypeForget = "FORGET";
    public const string TypeAnonymize = "ANONYMIZE";
    public const string TypeRetentionReview = "RETENTION_REVIEW";

    public const string StatusPending = "PENDING";
    public const string StatusInProgress = "IN_PROGRESS";
    public const string StatusCompleted = "COMPLETED";
    public const string StatusFailed = "FAILED";
    public const string StatusCanceled = "CANCELED";

    public long TenantId { get; private set; }
    public long CustomerId { get; private set; }
    public string RequestType { get; private set; } = string.Empty;
    public string Status { get; private set; } = string.Empty;
    public string? Reason { get; private set; }
    public long RequestedBy { get; private set; }
    public DateTime RequestedAt { get; private set; }
    public long? ProcessedBy { get; private set; }
    public DateTime? ProcessedAt { get; private set; }
    public string? FailureCode { get; private set; }
    public string? FailureReason { get; private set; }
    public string? CorrelationId { get; private set; }
    public byte[] RowVersion { get; private set; } = Array.Empty<byte>();

    long ITenantScopedEntity.TenantId
    {
        get => TenantId;
        set => TenantId = value;
    }

    private CustomerPrivacyRequest()
    {
    }

    public static CustomerPrivacyRequest Create(long tenantId, long customerId, string requestType, long requestedBy,
        string? reason, string? correlationId, DateTime? requestedAt = null)
    {
        ValidateTenantAndCustomer(tenantId, customerId);
        ValidateRequestType(requestType);
        if (requestedBy <= 0)
        {
            throw new InvalidOperationException("RequestedBy must be a positive value.");
        }

        return new CustomerPrivacyRequest
        {
            TenantId = tenantId,
            CustomerId = customerId,
            RequestType = requestType.Trim().ToUpperInvariant(),
            Status = StatusPending,
            Reason = NormalizeOrNull(reason),
            RequestedBy = requestedBy,
            RequestedAt = requestedAt ?? DateTime.UtcNow,
            CorrelationId = NormalizeOrNull(correlationId)
        };
    }

    public void MarkInProgress(long processedBy)
    {
        if (!string.Equals(Status, StatusPending, StringComparison.Ordinal))
        {
            throw new InvalidOperationException("Only pending requests can transition to in-progress.");
        }

        if (processedBy <= 0)
        {
            throw new InvalidOperationException("ProcessedBy must be a positive value.");
        }

        Status = StatusInProgress;
        ProcessedBy = processedBy;
        ProcessedAt = DateTime.UtcNow;
        FailureCode = null;
        FailureReason = null;
    }

    public void MarkCompleted(long processedBy)
    {
        if (!string.Equals(Status, StatusInProgress, StringComparison.Ordinal))
        {
            throw new InvalidOperationException("Only in-progress requests can be completed.");
        }

        if (processedBy <= 0)
        {
            throw new InvalidOperationException("ProcessedBy must be a positive value.");
        }

        Status = StatusCompleted;
        ProcessedBy = processedBy;
        ProcessedAt = DateTime.UtcNow;
        FailureCode = null;
        FailureReason = null;
    }

    public void MarkFailed(long processedBy, string failureCode, string? failureReason)
    {
        if (!string.Equals(Status, StatusInProgress, StringComparison.Ordinal))
        {
            throw new InvalidOperationException("Only in-progress requests can be marked as failed.");
        }

        if (processedBy <= 0)
        {
            throw new InvalidOperationException("ProcessedBy must be a positive value.");
        }

        if (string.IsNullOrWhiteSpace(failureCode))
        {
            throw new InvalidOperationException("FailureCode is required.");
        }

        Status = StatusFailed;
        ProcessedBy = processedBy;
        ProcessedAt = DateTime.UtcNow;
        FailureCode = failureCode.Trim().ToUpperInvariant();
        FailureReason = NormalizeOrNull(failureReason);
    }

    public void Cancel(long processedBy, string? reason)
    {
        if (!string.Equals(Status, StatusPending, StringComparison.Ordinal) &&
            !string.Equals(Status, StatusInProgress, StringComparison.Ordinal))
        {
            throw new InvalidOperationException("Only pending or in-progress requests can be canceled.");
        }

        if (processedBy <= 0)
        {
            throw new InvalidOperationException("ProcessedBy must be a positive value.");
        }

        Status = StatusCanceled;
        ProcessedBy = processedBy;
        ProcessedAt = DateTime.UtcNow;
        FailureCode = "CANCELED";
        FailureReason = NormalizeOrNull(reason);
    }

    public void MarkPendingForRetry(long processedBy, string? reason = null)
    {
        if (!string.Equals(Status, StatusFailed, StringComparison.Ordinal))
        {
            throw new InvalidOperationException("Only failed requests can be retried.");
        }

        if (processedBy <= 0)
        {
            throw new InvalidOperationException("ProcessedBy must be a positive value.");
        }

        Status = StatusPending;
        ProcessedBy = processedBy;
        ProcessedAt = DateTime.UtcNow;
        FailureCode = null;
        FailureReason = null;
        if (!string.IsNullOrWhiteSpace(reason))
        {
            Reason = NormalizeOrNull(reason);
        }
    }

    private static void ValidateTenantAndCustomer(long tenantId, long customerId)
    {
        if (tenantId <= 0)
        {
            throw new InvalidOperationException("TenantId must be a positive value.");
        }

        if (customerId <= 0)
        {
            throw new InvalidOperationException("CustomerId must be a positive value.");
        }
    }

    private static void ValidateRequestType(string requestType)
    {
        var normalized = requestType?.Trim().ToUpperInvariant();
        if (normalized is not (TypeForget or TypeAnonymize or TypeRetentionReview))
        {
            throw new InvalidOperationException("RequestType is invalid.");
        }
    }

    private static string? NormalizeOrNull(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
