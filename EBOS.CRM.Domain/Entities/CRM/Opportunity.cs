using EBOS.Core.Primitives;
using EBOS.CRM.Domain.Events;
using EBOS.CRM.Domain.Exceptions;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;

namespace EBOS.CRM.Domain.Entities.CRM;

public class Opportunity : ErasableEntity, ITenantScopedEntity
{
    private readonly DomainOperationalEventBuffer _operationalEvents = new();

    public long TenantId { get; set; }
    public string Name { get; set; } = null!;
    public long StageId { get; set; }
    public OpportunityStage Stage { get; set; } = null!;
    public long OwnerUserId { get; set; }
    public long CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
    public DateTime? ExpectedCloseDate { get; set; }
    public decimal Amount { get; set; }
    public decimal Probability { get; set; }
    public string? Source { get; set; }
    public long? SourceLeadId { get; set; }
    public Lead? SourceLead { get; set; }
    public string? CloseReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public long CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public long? UpdatedBy { get; set; }

    public ICollection<Quote> Quotes { get; set; } = new List<Quote>();

    public IReadOnlyCollection<DomainOperationalEvent> PeekOperationalEvents()
        => _operationalEvents.Peek();

    public IReadOnlyCollection<DomainOperationalEvent> DequeueOperationalEvents()
        => _operationalEvents.Dequeue();

    public void ApplyUpdate(
        string name,
        long stageId,
        long ownerUserId,
        long customerId,
        DateTime? expectedCloseDate,
        decimal amount,
        decimal probability,
        string? source,
        long? sourceLeadId,
        string? closeReason)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainValidationException("Name is required.", "DOMAIN_VALIDATION_OPPORTUNITY_NAME_REQUIRED");
        }

        if (ownerUserId <= 0 || customerId <= 0 || stageId <= 0)
        {
            throw new DomainValidationException("StageId, OwnerUserId and CustomerId must be positive values.", "DOMAIN_VALIDATION_OPPORTUNITY_IDS_POSITIVE");
        }

        if (amount < 0)
        {
            throw new DomainValidationException("Amount cannot be negative.", "DOMAIN_VALIDATION_OPPORTUNITY_AMOUNT_NON_NEGATIVE");
        }

        if (probability < 0 || probability > 1)
        {
            throw new DomainValidationException("Probability must be between 0 and 1.", "DOMAIN_VALIDATION_OPPORTUNITY_PROBABILITY_RANGE");
        }

        Name = name.Trim();
        OwnerUserId = ownerUserId;
        CustomerId = customerId;
        ExpectedCloseDate = expectedCloseDate;
        Amount = amount;
        Probability = probability;
        Source = string.IsNullOrWhiteSpace(source) ? null : source.Trim();
        SourceLeadId = sourceLeadId;
        CloseReason = string.IsNullOrWhiteSpace(closeReason) ? null : closeReason.Trim();

        SetStage(stageId);
    }

    public void SetStage(long stageId, decimal? probability = null)
    {
        if (stageId <= 0)
        {
            throw new DomainValidationException("StageId must be a positive value.", "DOMAIN_VALIDATION_OPPORTUNITY_STAGE_ID_POSITIVE");
        }

        if (probability.HasValue && (probability.Value < 0 || probability.Value > 1))
        {
            throw new DomainValidationException("Probability must be between 0 and 1.", "DOMAIN_VALIDATION_OPPORTUNITY_PROBABILITY_RANGE");
        }

        var targetProbability = probability ?? Probability;
        if (StageId == stageId && (!probability.HasValue || Probability == targetProbability))
        {
            _operationalEvents.Emit(
                "DomainCommandDeduplicated",
                new Dictionary<string, string>(StringComparer.Ordinal)
                {
                    ["aggregate"] = nameof(Opportunity),
                    ["command"] = nameof(SetStage),
                    ["stageId"] = stageId.ToString()
                });
            return;
        }

        StageId = stageId;
        if (probability.HasValue)
        {
            Probability = probability.Value;
        }

        _operationalEvents.Emit(
            "OpportunityStageChanged",
            new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["opportunityId"] = Id.ToString(),
                ["stageId"] = StageId.ToString()
            });
    }

    public void Close(long stageId, bool isWon, string? closeReason)
    {
        if (Probability is 0m or 1m && !string.IsNullOrWhiteSpace(CloseReason))
        {
            _operationalEvents.Emit(
                "DomainCommandDeduplicated",
                new Dictionary<string, string>(StringComparer.Ordinal)
                {
                    ["aggregate"] = nameof(Opportunity),
                    ["command"] = nameof(Close),
                    ["stageId"] = stageId.ToString()
                });
            return;
        }

        SetStage(stageId);
        Probability = isWon ? 1m : 0m;
        CloseReason = string.IsNullOrWhiteSpace(closeReason) ? null : closeReason.Trim();

        _operationalEvents.Emit(
            "OpportunityClosed",
            new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["opportunityId"] = Id.ToString(),
                ["isWon"] = isWon.ToString()
            });
    }
}
