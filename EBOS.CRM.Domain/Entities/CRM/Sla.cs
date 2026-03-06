using EBOS.Core.Primitives;
using EBOS.CRM.Domain.Events;
using EBOS.CRM.Domain.Exceptions;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;

namespace EBOS.CRM.Domain.Entities.CRM;

public class Sla : ErasableEntity, ITenantScopedEntity
{
    private readonly DomainOperationalEventBuffer _operationalEvents = new();

    public long TenantId { get; set; }
    public string Name { get; set; } = null!;
    public int TargetMinutes { get; set; }
    public int? WarningMinutes { get; set; }
    public DateTime? ActiveFrom { get; set; }
    public DateTime? ActiveTo { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public long CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public long? UpdatedBy { get; set; }

    public ICollection<Case> Cases { get; set; } = new List<Case>();

    public IReadOnlyCollection<DomainOperationalEvent> PeekOperationalEvents()
        => _operationalEvents.Peek();

    public IReadOnlyCollection<DomainOperationalEvent> DequeueOperationalEvents()
        => _operationalEvents.Dequeue();

    public bool IsActiveAt(DateTime date)
    {
        if (!IsActive)
        {
            return false;
        }

        if (ActiveFrom.HasValue && date < ActiveFrom.Value)
        {
            return false;
        }

        if (ActiveTo.HasValue && date > ActiveTo.Value)
        {
            return false;
        }

        return true;
    }

    public DateTime CalculateDueAt(DateTime start)
    {
        if (TargetMinutes <= 0)
        {
            throw new DomainValidationException("TargetMinutes must be greater than zero.", "DOMAIN_VALIDATION_TARGET_MINUTES_POSITIVE");
        }

        return start.AddMinutes(TargetMinutes);
    }

    public void ValidateWarningMinutes()
    {
        if (WarningMinutes.HasValue && WarningMinutes.Value < 0)
        {
            throw new DomainValidationException("WarningMinutes cannot be negative.", "DOMAIN_VALIDATION_WARNING_MINUTES_NON_NEGATIVE");
        }

        if (WarningMinutes.HasValue && WarningMinutes.Value > TargetMinutes)
        {
            throw new DomainValidationException("WarningMinutes cannot exceed TargetMinutes.", "DOMAIN_VALIDATION_WARNING_MINUTES_RANGE");
        }
    }

    public void ValidateActiveRange()
    {
        if (ActiveFrom.HasValue && ActiveTo.HasValue && ActiveFrom.Value > ActiveTo.Value)
        {
            throw new DomainValidationException("ActiveFrom cannot be later than ActiveTo.", "DOMAIN_VALIDATION_ACTIVE_RANGE");
        }
    }

    public bool IsBreached(DateTime now, DateTime? dueAt)
    {
        if (!dueAt.HasValue)
        {
            return false;
        }

        return now > dueAt.Value;
    }

    public void ToggleActive(bool isActive, bool hasOpenCases)
    {
        if (!isActive && hasOpenCases)
        {
            _operationalEvents.Emit(
                "DomainInvariantBreachDetected",
                new Dictionary<string, string>(StringComparer.Ordinal)
                {
                    ["aggregate"] = nameof(Sla),
                    ["slaId"] = Id.ToString(),
                    ["reason"] = "open_cases_prevent_deactivation"
                });
            throw new DomainRuleViolationException(
                "SLA has open cases and cannot be deactivated.",
                "DOMAIN_RULE_VIOLATION_SLA_HAS_OPEN_CASES");
        }

        if (IsActive == isActive)
        {
            EmitDedup(nameof(ToggleActive), "isActive", isActive.ToString());
            return;
        }

        IsActive = isActive;
        _operationalEvents.Emit(
            "SlaActivationChanged",
            new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["slaId"] = Id.ToString(),
                ["isActive"] = IsActive.ToString()
            });
    }

    private void EmitDedup(string command, string key, string value)
    {
        _operationalEvents.Emit(
            "DomainCommandDeduplicated",
            new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["aggregate"] = nameof(Sla),
                ["command"] = command,
                [key] = value
            });
    }
}
