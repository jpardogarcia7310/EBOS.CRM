using EBOS.Core.Primitives;
using EBOS.CRM.Domain.Events;
using EBOS.CRM.Domain.Exceptions;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;

namespace EBOS.CRM.Domain.Entities.CRM;

public class Quote : ErasableEntity, ITenantScopedEntity
{
    private readonly DomainOperationalEventBuffer _operationalEvents = new();

    public long TenantId { get; set; }
    public long OpportunityId { get; set; }
    public Opportunity Opportunity { get; set; } = null!;
    public string Status { get; set; } = null!;
    public string? ReferenceNumber { get; set; }
    public decimal SubtotalAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime? ValidUntil { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public long CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public long? UpdatedBy { get; set; }

    public IReadOnlyCollection<DomainOperationalEvent> PeekOperationalEvents()
        => _operationalEvents.Peek();

    public IReadOnlyCollection<DomainOperationalEvent> DequeueOperationalEvents()
        => _operationalEvents.Dequeue();

    public void ApplyUpdate(
        long opportunityId,
        string status,
        string? referenceNumber,
        decimal subtotalAmount,
        decimal discountAmount,
        decimal totalAmount,
        DateTime? validUntil,
        string? notes)
    {
        if (opportunityId <= 0)
        {
            throw new DomainValidationException("OpportunityId must be a positive value.", "DOMAIN_VALIDATION_QUOTE_OPPORTUNITY_ID_POSITIVE");
        }

        ValidateAmounts(subtotalAmount, discountAmount, totalAmount);

        OpportunityId = opportunityId;
        SetStatus(status);
        ReferenceNumber = string.IsNullOrWhiteSpace(referenceNumber) ? null : referenceNumber.Trim();
        SubtotalAmount = subtotalAmount;
        DiscountAmount = discountAmount;
        TotalAmount = totalAmount;
        ValidUntil = validUntil;
        Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
    }

    public void SetStatus(string status)
    {
        var normalized = NormalizeStatus(status);
        if (string.IsNullOrWhiteSpace(normalized))
        {
            throw new DomainValidationException("Status is required.", "DOMAIN_VALIDATION_QUOTE_STATUS_REQUIRED");
        }

        if (string.Equals(Status, normalized, StringComparison.Ordinal))
        {
            _operationalEvents.Emit(
                "DomainCommandDeduplicated",
                new Dictionary<string, string>(StringComparer.Ordinal)
                {
                    ["aggregate"] = nameof(Quote),
                    ["command"] = nameof(SetStatus),
                    ["status"] = normalized
                });
            return;
        }

        Status = normalized;
        _operationalEvents.Emit(
            "QuoteStatusChanged",
            new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["quoteId"] = Id.ToString(),
                ["status"] = Status
            });
    }

    private static void ValidateAmounts(decimal subtotalAmount, decimal discountAmount, decimal totalAmount)
    {
        if (subtotalAmount < 0 || discountAmount < 0 || totalAmount < 0)
        {
            throw new DomainValidationException("Quote amounts cannot be negative.", "DOMAIN_VALIDATION_QUOTE_AMOUNTS_NON_NEGATIVE");
        }

        if (discountAmount > subtotalAmount)
        {
            throw new DomainRuleViolationException("DiscountAmount cannot exceed SubtotalAmount.", "DOMAIN_RULE_VIOLATION_QUOTE_DISCOUNT_RANGE");
        }

        var expected = subtotalAmount - discountAmount;
        if (totalAmount != expected)
        {
            throw new DomainRuleViolationException("TotalAmount must equal SubtotalAmount minus DiscountAmount.", "DOMAIN_RULE_VIOLATION_QUOTE_TOTAL_MISMATCH");
        }
    }

    private static string NormalizeStatus(string? status)
        => string.IsNullOrWhiteSpace(status) ? string.Empty : status.Trim().ToUpperInvariant();
}
