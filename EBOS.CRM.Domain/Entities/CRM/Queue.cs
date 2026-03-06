using EBOS.Core.Primitives;
using EBOS.CRM.Domain.Events;
using EBOS.CRM.Domain.Exceptions;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;

namespace EBOS.CRM.Domain.Entities.CRM;

public class Queue : ErasableEntity, ITenantScopedEntity
{
    private readonly DomainOperationalEventBuffer _operationalEvents = new();

    public long TenantId { get; set; }
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;
    public bool IsActive { get; set; }
    public long? DefaultOwnerUserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public long CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public long? UpdatedBy { get; set; }

    public ICollection<Case> Cases { get; set; } = new List<Case>();

    public IReadOnlyCollection<DomainOperationalEvent> PeekOperationalEvents()
        => _operationalEvents.Peek();

    public IReadOnlyCollection<DomainOperationalEvent> DequeueOperationalEvents()
        => _operationalEvents.Dequeue();

    public void ToggleActive(bool isActive, bool hasOpenCases)
    {
        if (!isActive && hasOpenCases)
        {
            _operationalEvents.Emit(
                "DomainInvariantBreachDetected",
                new Dictionary<string, string>(StringComparer.Ordinal)
                {
                    ["aggregate"] = nameof(Queue),
                    ["queueId"] = Id.ToString(),
                    ["reason"] = "open_cases_prevent_deactivation"
                });
            throw new DomainRuleViolationException(
                "Queue has open cases and cannot be deactivated.",
                "DOMAIN_RULE_VIOLATION_QUEUE_HAS_OPEN_CASES");
        }

        if (IsActive == isActive)
        {
            EmitDedup(nameof(ToggleActive), "isActive", isActive.ToString());
            return;
        }

        IsActive = isActive;
        _operationalEvents.Emit(
            "QueueActivationChanged",
            new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["queueId"] = Id.ToString(),
                ["isActive"] = IsActive.ToString()
            });
    }

    public void AssignDefaultOwner(long? defaultOwnerUserId)
    {
        if (defaultOwnerUserId.HasValue && defaultOwnerUserId.Value <= 0)
        {
            throw new DomainValidationException(
                "DefaultOwnerUserId must be a positive value when provided.",
                "DOMAIN_VALIDATION_QUEUE_DEFAULT_OWNER_POSITIVE");
        }

        if (DefaultOwnerUserId == defaultOwnerUserId)
        {
            EmitDedup(nameof(AssignDefaultOwner), "defaultOwnerUserId", defaultOwnerUserId?.ToString() ?? "NULL");
            return;
        }

        DefaultOwnerUserId = defaultOwnerUserId;
        _operationalEvents.Emit(
            "QueueDefaultOwnerAssigned",
            new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["queueId"] = Id.ToString(),
                ["defaultOwnerUserId"] = DefaultOwnerUserId?.ToString() ?? "UNASSIGNED"
            });
    }

    private void EmitDedup(string command, string key, string value)
    {
        _operationalEvents.Emit(
            "DomainCommandDeduplicated",
            new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["aggregate"] = nameof(Queue),
                ["command"] = command,
                [key] = value
            });
    }
}
