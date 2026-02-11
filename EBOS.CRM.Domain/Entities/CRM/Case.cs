using System;
using EBOS.Core.Primitives;
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

    public void AssignQueue(long queueId)
    {
        if (queueId <= 0)
        {
            throw new InvalidOperationException("QueueId must be a positive value.");
        }

        QueueId = queueId;
    }

    public void AssignOwner(long ownerUserId)
    {
        if (ownerUserId <= 0)
        {
            throw new InvalidOperationException("OwnerUserId must be a positive value.");
        }

        OwnerUserId = ownerUserId;
    }

    public void Close(DateTime closedAt)
    {
        if (ClosedAt.HasValue || string.Equals(Status, StatusClosed, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Case is already closed.");
        }

        SetStatus(StatusClosed);
        ClosedAt = closedAt;
    }

    public void Reopen()
    {
        if (!ClosedAt.HasValue && !string.Equals(Status, StatusClosed, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Case is not closed.");
        }

        SetStatus(StatusReopened);
        ClosedAt = null;
    }

    public void UpdateDueAt(DateTime? dueAt)
    {
        if (dueAt.HasValue && dueAt.Value < CreatedAt)
        {
            throw new InvalidOperationException("DueAt cannot be earlier than CreatedAt.");
        }

        DueAt = dueAt;
    }

    public void SetPriority(string priority)
    {
        if (!IsValidPriority(priority))
        {
            throw new InvalidOperationException("Priority value is invalid.");
        }

        Priority = priority;
    }

    public void SetStatus(string status)
    {
        if (!IsValidStatus(status))
        {
            throw new InvalidOperationException("Status value is invalid.");
        }

        if (!IsValidTransition(Status, status))
        {
            throw new InvalidOperationException("Status transition is not allowed.");
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
