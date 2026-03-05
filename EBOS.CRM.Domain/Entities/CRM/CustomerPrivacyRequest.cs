using EBOS.Core.Primitives;
using EBOS.CRM.Domain.Exceptions;
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
            throw new DomainValidationException("RequestedBy must be a positive value.", "DOMAIN_VALIDATION_REQUESTED_BY_POSITIVE");
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
            throw new DomainRuleViolationException("Only pending requests can transition to in-progress.", "DOMAIN_RULE_VIOLATION_PRIVACY_REQUEST_TRANSITION_IN_PROGRESS");
        }

        if (processedBy <= 0)
        {
            throw new DomainValidationException("ProcessedBy must be a positive value.", "DOMAIN_VALIDATION_PROCESSED_BY_POSITIVE");
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
            throw new DomainRuleViolationException("Only in-progress requests can be completed.", "DOMAIN_RULE_VIOLATION_PRIVACY_REQUEST_TRANSITION_COMPLETED");
        }

        if (processedBy <= 0)
        {
            throw new DomainValidationException("ProcessedBy must be a positive value.", "DOMAIN_VALIDATION_PROCESSED_BY_POSITIVE");
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
            throw new DomainRuleViolationException("Only in-progress requests can be marked as failed.", "DOMAIN_RULE_VIOLATION_PRIVACY_REQUEST_TRANSITION_FAILED");
        }

        if (processedBy <= 0)
        {
            throw new DomainValidationException("ProcessedBy must be a positive value.", "DOMAIN_VALIDATION_PROCESSED_BY_POSITIVE");
        }

        if (string.IsNullOrWhiteSpace(failureCode))
        {
            throw new DomainValidationException("FailureCode is required.", "DOMAIN_VALIDATION_FAILURE_CODE_REQUIRED");
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
            throw new DomainRuleViolationException("Only pending or in-progress requests can be canceled.", "DOMAIN_RULE_VIOLATION_PRIVACY_REQUEST_TRANSITION_CANCELED");
        }

        if (processedBy <= 0)
        {
            throw new DomainValidationException("ProcessedBy must be a positive value.", "DOMAIN_VALIDATION_PROCESSED_BY_POSITIVE");
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
            throw new DomainRuleViolationException("Only failed requests can be retried.", "DOMAIN_RULE_VIOLATION_PRIVACY_REQUEST_TRANSITION_RETRY");
        }

        if (processedBy <= 0)
        {
            throw new DomainValidationException("ProcessedBy must be a positive value.", "DOMAIN_VALIDATION_PROCESSED_BY_POSITIVE");
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

    public bool MatchesRegistrationIntent(string requestType, string? reason, long requestedBy)
    {
        var normalizedType = requestType?.Trim().ToUpperInvariant() ?? string.Empty;
        var normalizedReason = NormalizeOrNull(reason);
        return TenantId > 0
               && requestedBy > 0
               && RequestedBy == requestedBy
               && string.Equals(RequestType, normalizedType, StringComparison.Ordinal)
               && string.Equals(Reason, normalizedReason, StringComparison.Ordinal);
    }

    private static void ValidateTenantAndCustomer(long tenantId, long customerId)
    {
        if (tenantId <= 0)
        {
            throw new DomainValidationException("TenantId must be a positive value.", "DOMAIN_VALIDATION_TENANT_ID_POSITIVE");
        }

        if (customerId <= 0)
        {
            throw new DomainValidationException("CustomerId must be a positive value.", "DOMAIN_VALIDATION_CUSTOMER_ID_POSITIVE");
        }
    }

    private static void ValidateRequestType(string requestType)
    {
        var normalized = requestType?.Trim().ToUpperInvariant();
        if (normalized is not (TypeForget or TypeAnonymize or TypeRetentionReview))
        {
            throw new DomainValidationException("RequestType is invalid.", "DOMAIN_VALIDATION_REQUEST_TYPE_INVALID");
        }
    }

    private static string? NormalizeOrNull(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
