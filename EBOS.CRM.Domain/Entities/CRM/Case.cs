using EBOS.Core.Primitives;
using EBOS.CRM.Domain.Exceptions;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;

namespace EBOS.CRM.Domain.Entities.CRM;

public class Case : ErasableEntity, ITenantScopedEntity
{
    public const string StatusOpen = "Open";
    public const string StatusInProgress = "InProgress";
    public const string StatusOnHold = "OnHold";
    public const string StatusResolved = "Resolved";
    public const string StatusClosed = "Closed";
    public const string StatusReopened = "Reopened";

    public const string PriorityLow = "Low";
    public const string PriorityMedium = "Medium";
    public const string PriorityHigh = "High";
    public const string PriorityUrgent = "Urgent";

    public long TenantId { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public string Status { get; set; } = null!;
    public string Priority { get; set; } = null!;
    public long OwnerUserId { get; set; }
    public long QueueId { get; set; }
    public Queue Queue { get; set; } = null!;
    public long SlaId { get; set; }
    public Sla Sla { get; set; } = null!;
    public DateTime? DueAt { get; set; }
    public DateTime? ClosedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public long CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public long? UpdatedBy { get; set; }

    public ICollection<CaseActivity> Activities { get; set; } = new List<CaseActivity>();

    public void Open()
    {
        if (!string.IsNullOrWhiteSpace(Status))
        {
            throw new DomainRuleViolationException("Case is already initialized.", "DOMAIN_RULE_VIOLATION_CASE_ALREADY_INITIALIZED");
        }

        Status = StatusOpen;
    }

    public void UpdateDetails(string title, string? description)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new DomainValidationException("Title is required.", "DOMAIN_VALIDATION_CASE_TITLE_REQUIRED");
        }

        Title = title;
        Description = description;
    }

    public void AssignQueue(long queueId)
    {
        if (queueId <= 0)
        {
            throw new DomainValidationException("QueueId must be a positive value.", "DOMAIN_VALIDATION_QUEUE_ID_POSITIVE");
        }

        QueueId = queueId;
    }

    public void AssignOwner(long ownerUserId)
    {
        if (ownerUserId <= 0)
        {
            throw new DomainValidationException("OwnerUserId must be a positive value.", "DOMAIN_VALIDATION_OWNER_USER_ID_POSITIVE");
        }

        OwnerUserId = ownerUserId;
    }

    public void Close(DateTime closedAt)
    {
        if (ClosedAt.HasValue || string.Equals(Status, StatusClosed, StringComparison.OrdinalIgnoreCase))
        {
            throw new DomainRuleViolationException("Case is already closed.", "DOMAIN_RULE_VIOLATION_CASE_ALREADY_CLOSED");
        }

        SetStatus(StatusClosed);
        ClosedAt = closedAt;
    }

    public void Reopen()
    {
        if (!ClosedAt.HasValue && !string.Equals(Status, StatusClosed, StringComparison.OrdinalIgnoreCase))
        {
            throw new DomainRuleViolationException("Case is not closed.", "DOMAIN_RULE_VIOLATION_CASE_NOT_CLOSED");
        }

        SetStatus(StatusReopened);
        ClosedAt = null;
    }

    public void UpdateDueAt(DateTime? dueAt)
    {
        if (dueAt.HasValue && dueAt.Value < CreatedAt)
        {
            throw new DomainValidationException("DueAt cannot be earlier than CreatedAt.", "DOMAIN_VALIDATION_DUE_AT_RANGE");
        }

        DueAt = dueAt;
    }

    public void SetPriority(string priority)
    {
        if (!IsValidPriority(priority))
        {
            throw new DomainValidationException("Priority value is invalid.", "DOMAIN_VALIDATION_CASE_PRIORITY_INVALID");
        }

        Priority = priority;
    }

    public void SetStatus(string status)
    {
        if (!IsValidStatus(status))
        {
            throw new DomainValidationException("Status value is invalid.", "DOMAIN_VALIDATION_CASE_STATUS_INVALID");
        }

        if (!IsValidTransition(Status, status))
        {
            throw new DomainRuleViolationException("Status transition is not allowed.", "DOMAIN_RULE_VIOLATION_CASE_STATUS_TRANSITION");
        }

        Status = status;
    }

    public static bool IsValidStatus(string status)
    {
        return status is StatusOpen or StatusInProgress or StatusOnHold or StatusResolved or StatusClosed or StatusReopened;
    }

    public static bool IsValidPriority(string priority)
    {
        return priority is PriorityLow or PriorityMedium or PriorityHigh or PriorityUrgent;
    }

    public static bool IsValidTransition(string currentStatus, string nextStatus)
    {
        if (string.IsNullOrWhiteSpace(currentStatus))
        {
            return nextStatus == StatusOpen;
        }

        return currentStatus switch
        {
            StatusOpen => nextStatus is StatusInProgress or StatusOnHold or StatusResolved or StatusClosed,
            StatusInProgress => nextStatus is StatusOnHold or StatusResolved or StatusClosed,
            StatusOnHold => nextStatus is StatusInProgress or StatusResolved or StatusClosed,
            StatusResolved => nextStatus is StatusClosed or StatusReopened,
            StatusClosed => nextStatus == StatusReopened,
            StatusReopened => nextStatus is StatusInProgress or StatusOnHold or StatusResolved or StatusClosed,
            _ => false
        };
    }
}
