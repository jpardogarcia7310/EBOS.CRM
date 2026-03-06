using EBOS.Core.Primitives;
using EBOS.CRM.Domain.Events;
using EBOS.CRM.Domain.Exceptions;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;

namespace EBOS.CRM.Domain.Entities.CRM;

public class CaseActivity : ErasableEntity, ITenantScopedEntity
{
    private readonly DomainOperationalEventBuffer _operationalEvents = new();

    public const string StatusOpen = "Open";
    public const string StatusInProgress = "InProgress";
    public const string StatusCompleted = "Completed";
    public const string StatusCancelled = "Cancelled";

    public long TenantId { get; set; }
    public long CaseId { get; set; }
    public Case Case { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public string Status { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public long CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public long? UpdatedBy { get; set; }

    public IReadOnlyCollection<DomainOperationalEvent> PeekOperationalEvents()
        => _operationalEvents.Peek();

    public IReadOnlyCollection<DomainOperationalEvent> DequeueOperationalEvents()
        => _operationalEvents.Dequeue();

    public void SetStatus(string status)
    {
        if (!IsValidStatus(status))
        {
            throw new DomainValidationException("Status value is invalid.", "DOMAIN_VALIDATION_CASE_ACTIVITY_STATUS_INVALID");
        }

        if (string.Equals(Status, status, StringComparison.Ordinal))
        {
            EmitOperationalEvent(
                "DomainCommandDeduplicated",
                new Dictionary<string, string>(StringComparer.Ordinal)
                {
                    ["aggregate"] = nameof(CaseActivity),
                    ["command"] = nameof(SetStatus),
                    ["status"] = status
                });
            return;
        }

        if (!IsValidTransition(Status, status))
        {
            EmitOperationalEvent(
                "DomainInvariantBreachDetected",
                new Dictionary<string, string>(StringComparer.Ordinal)
                {
                    ["aggregate"] = nameof(CaseActivity),
                    ["command"] = nameof(SetStatus),
                    ["currentStatus"] = Status,
                    ["targetStatus"] = status
                });
            throw new DomainRuleViolationException("Status transition is not allowed.", "DOMAIN_RULE_VIOLATION_CASE_ACTIVITY_STATUS_TRANSITION");
        }

        var previousStatus = string.IsNullOrWhiteSpace(Status) ? "<UNINITIALIZED>" : Status;
        Status = status;
        EmitOperationalEvent(
            "CaseActivityStatusChanged",
            new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["aggregate"] = nameof(CaseActivity),
                ["fromStatus"] = previousStatus,
                ["toStatus"] = status
            });
    }

    public static bool IsValidStatus(string status)
    {
        return status is StatusOpen or StatusInProgress or StatusCompleted or StatusCancelled;
    }

    public static bool IsValidTransition(string currentStatus, string nextStatus)
    {
        if (string.IsNullOrWhiteSpace(currentStatus))
        {
            return nextStatus == StatusOpen;
        }

        return currentStatus switch
        {
            StatusOpen => nextStatus is StatusInProgress or StatusCompleted or StatusCancelled,
            StatusInProgress => nextStatus is StatusCompleted or StatusCancelled,
            StatusCompleted => false,
            StatusCancelled => false,
            _ => false
        };
    }

    private void EmitOperationalEvent(string eventName, IReadOnlyDictionary<string, string>? evidence = null)
        => _operationalEvents.Emit(eventName, evidence);
}
