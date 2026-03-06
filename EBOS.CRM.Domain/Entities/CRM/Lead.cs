using EBOS.Core.Primitives;
using EBOS.CRM.Domain.Events;
using EBOS.CRM.Domain.Exceptions;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;

namespace EBOS.CRM.Domain.Entities.CRM;

public class Lead : ErasableEntity, ITenantScopedEntity
{
    private readonly DomainOperationalEventBuffer _operationalEvents = new();

    public const string StatusNew = "New";
    public const string StatusQualified = "Qualified";
    public const string StatusDisqualified = "Disqualified";
    public const string StatusConverted = "Converted";

    public long TenantId { get; set; }
    public string Source { get; set; } = null!;
    public string Status { get; set; } = null!;
    public long OwnerUserId { get; set; }
    public string CompanyName { get; set; } = null!;
    public string ContactName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public decimal? EstimatedValue { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public long CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public long? UpdatedBy { get; set; }

    public long? ConvertedOpportunityId { get; set; }
    public Opportunity? ConvertedOpportunity { get; set; }

    public IReadOnlyCollection<DomainOperationalEvent> PeekOperationalEvents()
        => _operationalEvents.Peek();

    public IReadOnlyCollection<DomainOperationalEvent> DequeueOperationalEvents()
        => _operationalEvents.Dequeue();

    public void ApplyUpdate(
        string source,
        string status,
        long ownerUserId,
        string companyName,
        string contactName,
        string email,
        string phone,
        decimal? estimatedValue,
        string? notes)
    {
        EnsureRequiredText(source, "Source", "DOMAIN_VALIDATION_LEAD_SOURCE_REQUIRED");
        EnsureRequiredText(companyName, "CompanyName", "DOMAIN_VALIDATION_LEAD_COMPANY_REQUIRED");
        EnsureRequiredText(contactName, "ContactName", "DOMAIN_VALIDATION_LEAD_CONTACT_REQUIRED");
        EnsureRequiredText(email, "Email", "DOMAIN_VALIDATION_LEAD_EMAIL_REQUIRED");
        EnsureRequiredText(phone, "Phone", "DOMAIN_VALIDATION_LEAD_PHONE_REQUIRED");

        if (ownerUserId <= 0)
        {
            throw new DomainValidationException("OwnerUserId must be a positive value.", "DOMAIN_VALIDATION_LEAD_OWNER_POSITIVE");
        }

        if (estimatedValue.HasValue && estimatedValue.Value < 0)
        {
            throw new DomainValidationException("EstimatedValue cannot be negative.", "DOMAIN_VALIDATION_LEAD_ESTIMATED_VALUE_NON_NEGATIVE");
        }

        Source = source.Trim();
        OwnerUserId = ownerUserId;
        CompanyName = companyName.Trim();
        ContactName = contactName.Trim();
        Email = email.Trim();
        Phone = phone.Trim();
        EstimatedValue = estimatedValue;
        Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
        var normalized = NormalizeStatus(status);
        if (TryCanonicalStatus(normalized, out var canonical))
        {
            SetStatus(canonical);
        }
        else
        {
            if (string.IsNullOrWhiteSpace(status))
            {
                throw new DomainValidationException("Lead status is required.", "DOMAIN_VALIDATION_LEAD_STATUS_REQUIRED");
            }

            if (string.Equals(Status, status.Trim(), StringComparison.Ordinal))
            {
                EmitDedup(nameof(ApplyUpdate), "status", status.Trim());
                return;
            }

            Status = status.Trim();
            _operationalEvents.Emit(
                "LeadStatusChanged",
                new Dictionary<string, string>(StringComparer.Ordinal)
                {
                    ["leadId"] = Id.ToString(),
                    ["status"] = Status
                });
        }
    }

    public void Qualify(string? notes = null)
    {
        SetStatus(StatusQualified);
        if (!string.IsNullOrWhiteSpace(notes))
        {
            Notes = notes.Trim();
        }
    }

    public void Disqualify(string reason)
    {
        EnsureRequiredText(reason, "Reason", "DOMAIN_VALIDATION_LEAD_DISQUALIFY_REASON_REQUIRED");
        SetStatus(StatusDisqualified);
        Notes = reason.Trim();
    }

    public void MarkConverted(long convertedOpportunityId, string? notes = null)
    {
        if (ConvertedOpportunityId.HasValue && ConvertedOpportunityId.Value == convertedOpportunityId &&
            string.Equals(Status, StatusConverted, StringComparison.OrdinalIgnoreCase))
        {
            EmitDedup(nameof(MarkConverted), "convertedOpportunityId", convertedOpportunityId.ToString());
            return;
        }

        SetStatus(StatusConverted);
        if (convertedOpportunityId > 0)
        {
            ConvertedOpportunityId = convertedOpportunityId;
        }
        if (!string.IsNullOrWhiteSpace(notes))
        {
            Notes = notes.Trim();
        }

        _operationalEvents.Emit(
            "LeadConverted",
            new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["leadId"] = Id.ToString(),
                ["convertedOpportunityId"] = ConvertedOpportunityId?.ToString() ?? "UNASSIGNED"
            });
    }

    public void SetStatus(string status)
    {
        var normalized = NormalizeStatus(status);
        if (!TryCanonicalStatus(normalized, out var canonical))
        {
            throw new DomainValidationException("Lead status is invalid.", "DOMAIN_VALIDATION_LEAD_STATUS_INVALID");
        }

        var current = NormalizeStatus(Status);
        if (string.Equals(current, NormalizeStatus(canonical), StringComparison.Ordinal))
        {
            EmitDedup(nameof(SetStatus), "status", normalized);
            return;
        }

        if (!IsValidTransition(current, NormalizeStatus(canonical)))
        {
            _operationalEvents.Emit(
                "DomainInvariantBreachDetected",
                new Dictionary<string, string>(StringComparer.Ordinal)
                {
                    ["aggregate"] = nameof(Lead),
                    ["currentStatus"] = current,
                    ["targetStatus"] = canonical
                });
            throw new DomainRuleViolationException("Lead status transition is not allowed.", "DOMAIN_RULE_VIOLATION_LEAD_STATUS_TRANSITION");
        }

        Status = canonical;
        _operationalEvents.Emit(
            "LeadStatusChanged",
            new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["leadId"] = Id.ToString(),
                ["status"] = Status
            });
    }

    private static string NormalizeStatus(string? value)
        => string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim().ToUpperInvariant();

    private static bool TryCanonicalStatus(string normalized, out string canonical)
    {
        canonical = normalized switch
        {
            "NEW" => StatusNew,
            "QUALIFIED" => StatusQualified,
            "DISQUALIFIED" => StatusDisqualified,
            "CONVERTED" => StatusConverted,
            _ => string.Empty
        };

        return !string.IsNullOrEmpty(canonical);
    }

    private static bool IsValidTransition(string current, string next)
    {
        if (string.IsNullOrEmpty(current))
        {
            return next == "NEW";
        }

        return current switch
        {
            "NEW" => next is "QUALIFIED" or "DISQUALIFIED" or "CONVERTED",
            "QUALIFIED" => next is "DISQUALIFIED" or "CONVERTED",
            "DISQUALIFIED" => false,
            "CONVERTED" => false,
            _ => false
        };
    }

    private static void EnsureRequiredText(string? value, string field, string code)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainValidationException($"{field} is required.", code);
        }
    }

    private void EmitDedup(string command, string key, string value)
    {
        _operationalEvents.Emit(
            "DomainCommandDeduplicated",
            new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["aggregate"] = nameof(Lead),
                ["command"] = command,
                [key] = value
            });
    }
}
