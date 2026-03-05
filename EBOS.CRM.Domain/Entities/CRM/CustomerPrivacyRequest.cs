using EBOS.Core.Primitives;
using EBOS.CRM.Domain.Events;
using EBOS.CRM.Domain.Exceptions;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;

namespace EBOS.CRM.Domain.Entities.CRM;

public class CustomerPrivacyRequest : ErasableEntity, ITenantScopedEntity
{
    private readonly List<DomainOperationalEvent> _operationalEvents = [];

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

    public IReadOnlyCollection<DomainOperationalEvent> PeekOperationalEvents()
        => _operationalEvents.AsReadOnly();

    public IReadOnlyCollection<DomainOperationalEvent> DequeueOperationalEvents()
    {
        var snapshot = _operationalEvents.ToArray();
        _operationalEvents.Clear();
        return snapshot;
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

        var request = new CustomerPrivacyRequest
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

        request.EmitOperationalEvent(
            "CustomerPrivacyRequestRegistered",
            new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["tenantId"] = request.TenantId.ToString(),
                ["customerId"] = request.CustomerId.ToString(),
                ["requestType"] = request.RequestType
            });

        return request;
    }

    public void MarkInProgress(long processedBy)
    {
        if (string.Equals(Status, StatusInProgress, StringComparison.Ordinal))
        {
            // Idempotent retry guard: duplicated execution start must not duplicate side effects.
            EmitOperationalEvent(
                "DomainCommandDeduplicated",
                new Dictionary<string, string>(StringComparer.Ordinal)
                {
                    ["aggregate"] = nameof(CustomerPrivacyRequest),
                    ["command"] = nameof(MarkInProgress),
                    ["status"] = StatusInProgress
                });
            return;
        }

        EnsureMonotonicTransition(
            StatusInProgress,
            "DOMAIN_RULE_VIOLATION_PRIVACY_REQUEST_TRANSITION_IN_PROGRESS",
            "Only pending requests can transition to in-progress.");

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
        if (string.Equals(Status, StatusCompleted, StringComparison.Ordinal))
        {
            // Idempotent retry guard: repeated completion does not mutate state.
            EmitOperationalEvent(
                "DomainCommandDeduplicated",
                new Dictionary<string, string>(StringComparer.Ordinal)
                {
                    ["aggregate"] = nameof(CustomerPrivacyRequest),
                    ["command"] = nameof(MarkCompleted),
                    ["status"] = StatusCompleted
                });
            return;
        }

        EnsureMonotonicTransition(
            StatusCompleted,
            "DOMAIN_RULE_VIOLATION_PRIVACY_REQUEST_TRANSITION_COMPLETED",
            "Only in-progress requests can be completed.");

        if (processedBy <= 0)
        {
            throw new DomainValidationException("ProcessedBy must be a positive value.", "DOMAIN_VALIDATION_PROCESSED_BY_POSITIVE");
        }

        Status = StatusCompleted;
        ProcessedBy = processedBy;
        ProcessedAt = DateTime.UtcNow;
        FailureCode = null;
        FailureReason = null;
        EmitOperationalEvent(
            "CustomerPrivacyRequestCompleted",
            new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["tenantId"] = TenantId.ToString(),
                ["customerId"] = CustomerId.ToString(),
                ["requestType"] = RequestType
            });
    }

    public void MarkFailed(long processedBy, string failureCode, string? failureReason)
    {
        var normalizedCode = failureCode?.Trim().ToUpperInvariant();
        if (string.Equals(Status, StatusFailed, StringComparison.Ordinal) &&
            !string.IsNullOrWhiteSpace(normalizedCode) &&
            string.Equals(FailureCode, normalizedCode, StringComparison.Ordinal))
        {
            // Idempotent retry guard: same failure classification already applied.
            EmitOperationalEvent(
                "DomainCommandDeduplicated",
                new Dictionary<string, string>(StringComparer.Ordinal)
                {
                    ["aggregate"] = nameof(CustomerPrivacyRequest),
                    ["command"] = nameof(MarkFailed),
                    ["status"] = StatusFailed
                });
            return;
        }

        EnsureMonotonicTransition(
            StatusFailed,
            "DOMAIN_RULE_VIOLATION_PRIVACY_REQUEST_TRANSITION_FAILED",
            "Only in-progress requests can be marked as failed.");

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
        FailureCode = normalizedCode!;
        FailureReason = NormalizeOrNull(failureReason);
    }

    public void Cancel(long processedBy, string? reason)
    {
        if (string.Equals(Status, StatusCanceled, StringComparison.Ordinal))
        {
            // Idempotent retry guard: repeated cancellation is a no-op.
            EmitOperationalEvent(
                "DomainCommandDeduplicated",
                new Dictionary<string, string>(StringComparer.Ordinal)
                {
                    ["aggregate"] = nameof(CustomerPrivacyRequest),
                    ["command"] = nameof(Cancel),
                    ["status"] = StatusCanceled
                });
            return;
        }

        EnsureMonotonicTransition(
            StatusCanceled,
            "DOMAIN_RULE_VIOLATION_PRIVACY_REQUEST_TRANSITION_CANCELED",
            "Only pending or in-progress requests can be canceled.");

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
        CompensateToPendingForRetry(processedBy, reason);
    }

    public void CompensateToPendingForRetry(long processedBy, string? reason = null)
    {
        if (!string.Equals(Status, StatusFailed, StringComparison.Ordinal))
        {
            EmitOperationalEvent(
                "DomainInvariantBreachDetected",
                new Dictionary<string, string>(StringComparer.Ordinal)
                {
                    ["aggregate"] = nameof(CustomerPrivacyRequest),
                    ["command"] = nameof(CompensateToPendingForRetry),
                    ["currentStatus"] = Status,
                    ["requiredStatus"] = StatusFailed
                });
            throw new DomainRuleViolationException(
                "Only failed requests can be compensated back to pending for retry.",
                "DOMAIN_RULE_VIOLATION_PRIVACY_REQUEST_COMPENSATE_RETRY");
        }

        if (processedBy <= 0)
        {
            throw new DomainValidationException("ProcessedBy must be a positive value.", "DOMAIN_VALIDATION_PROCESSED_BY_POSITIVE");
        }

        // Explicit compensating action for reversible execution workflow.
        Status = StatusPending;
        ProcessedBy = processedBy;
        ProcessedAt = DateTime.UtcNow;
        FailureCode = null;
        FailureReason = null;
        if (!string.IsNullOrWhiteSpace(reason))
        {
            Reason = NormalizeOrNull(reason);
        }

        EmitOperationalEvent(
            "CustomerPrivacyRequestCompensationTriggered",
            new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["tenantId"] = TenantId.ToString(),
                ["customerId"] = CustomerId.ToString(),
                ["requestType"] = RequestType,
                ["fromStatus"] = StatusFailed,
                ["toStatus"] = StatusPending
            });
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

    private void EnsureMonotonicTransition(string targetStatus, string errorCode, string errorMessage)
    {
        if (IsAllowedMonotonicTransition(Status, targetStatus))
        {
            return;
        }

        EmitOperationalEvent(
            "DomainInvariantBreachDetected",
            new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["aggregate"] = nameof(CustomerPrivacyRequest),
                ["currentStatus"] = Status,
                ["targetStatus"] = targetStatus,
                ["invariant"] = "MONOTONIC_STATUS_TRANSITION"
            });

        throw new DomainRuleViolationException(errorMessage, errorCode);
    }

    private static bool IsAllowedMonotonicTransition(string currentStatus, string targetStatus)
        => (currentStatus, targetStatus) switch
        {
            (StatusPending, StatusInProgress) => true,
            (StatusPending, StatusCanceled) => true,
            (StatusInProgress, StatusCompleted) => true,
            (StatusInProgress, StatusFailed) => true,
            (StatusInProgress, StatusCanceled) => true,
            (StatusFailed, StatusPending) => true,
            _ => false
        };

    private void EmitOperationalEvent(string eventName, IReadOnlyDictionary<string, string>? evidence = null)
    {
        var category = DomainOperationalEventCatalog.Classify(eventName);
        _operationalEvents.Add(new DomainOperationalEvent(
            Name: eventName,
            Category: category,
            OccurredAtUtc: DateTime.UtcNow,
            Evidence: evidence ?? new Dictionary<string, string>(StringComparer.Ordinal)));
    }
}
