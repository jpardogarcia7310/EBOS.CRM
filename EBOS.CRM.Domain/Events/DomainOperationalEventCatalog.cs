namespace EBOS.CRM.Domain.Events;

public static class DomainOperationalEventCatalog
{
    public const string Concept =
        "Operational event taxonomy classifies domain events for analytics; it is a governance model, not domain entities.";

    public const bool RepresentsDomainEntities = false;

    public static IReadOnlyCollection<DomainOperationalEventDefinition> All { get; } =
    [
        new(
            "CustomerPrivacyRequestRegistered",
            DomainOperationalEventCategory.Business,
            "Customer privacy request accepted for domain processing.",
            "Track business operation throughput and completion funnel."),
        new(
            "DomainCommandDeduplicated",
            DomainOperationalEventCategory.Technical,
            "An idempotent command replay was safely ignored.",
            "Track retry pressure and deduplication effectiveness."),
        new(
            "DomainInvariantBreachDetected",
            DomainOperationalEventCategory.Anomaly,
            "Unexpected invariant breach observed during domain execution.",
            "Trigger anomaly triage and reliability investigations."),
        new(
            "DomainTransientFailureDetected",
            DomainOperationalEventCategory.Technical,
            "A transient domain failure was classified deterministically for retry handling.",
            "Track transient-domain failure pressure and retry triggers."),
        new(
            "CustomerPrivacyRequestCompensationTriggered",
            DomainOperationalEventCategory.Technical,
            "A failed privacy request was explicitly compensated back to pending for retry.",
            "Track compensation rate and validate reversible-domain behavior."),
        new(
            "CustomerPrivacyRequestCompleted",
            DomainOperationalEventCategory.Business,
            "A privacy request completed successfully.",
            "Track business completion outcomes for privacy workflows."),
        new(
            "CaseStatusChanged",
            DomainOperationalEventCategory.Business,
            "Case status changed through a valid domain transition.",
            "Track case workflow progression and business throughput."),
        new(
            "CaseOwnerAssigned",
            DomainOperationalEventCategory.Business,
            "A case owner assignment was applied.",
            "Track ownership changes and routing outcomes."),
        new(
            "CaseQueueAssigned",
            DomainOperationalEventCategory.Business,
            "A case queue assignment was applied.",
            "Track queue routing and work distribution."),
        new(
            "CaseSlaAssigned",
            DomainOperationalEventCategory.Business,
            "A case SLA assignment was applied.",
            "Track SLA governance and due-date recalculation."),
        new(
            "CaseActivityStatusChanged",
            DomainOperationalEventCategory.Business,
            "Case activity status changed through a valid transition.",
            "Track service workflow execution progress."),
        new(
            "CustomerConsentGranted",
            DomainOperationalEventCategory.Business,
            "A customer consent grant event was registered.",
            "Track consent acceptance rates and channel permissions."),
        new(
            "CustomerConsentRevoked",
            DomainOperationalEventCategory.Business,
            "A customer consent revocation event was registered.",
            "Track consent withdrawals and compliance impacts."),
        new(
            "AccountContactAssigned",
            DomainOperationalEventCategory.Business,
            "An account contact assignment was applied.",
            "Track account-contact relationship updates."),
        new(
            "AccountContactRoleChanged",
            DomainOperationalEventCategory.Business,
            "An account contact role state changed.",
            "Track role lifecycle and primary role changes."),
        new(
            "LeadStatusChanged",
            DomainOperationalEventCategory.Business,
            "Lead status changed through a valid domain transition.",
            "Track lead funnel progression and conversion readiness."),
        new(
            "LeadConverted",
            DomainOperationalEventCategory.Business,
            "Lead conversion to opportunity completed.",
            "Track conversion rate and conversion latency."),
        new(
            "OpportunityStageChanged",
            DomainOperationalEventCategory.Business,
            "Opportunity stage changed.",
            "Track pipeline movement and stage throughput."),
        new(
            "OpportunityClosed",
            DomainOperationalEventCategory.Business,
            "Opportunity closed as won or lost.",
            "Track close outcomes and forecast realization."),
        new(
            "QuoteStatusChanged",
            DomainOperationalEventCategory.Business,
            "Quote status changed.",
            "Track quote lifecycle and acceptance outcomes."),
        new(
            "QueueActivationChanged",
            DomainOperationalEventCategory.Business,
            "Queue active/inactive status changed.",
            "Track routing-capacity changes in service operations."),
        new(
            "QueueDefaultOwnerAssigned",
            DomainOperationalEventCategory.Business,
            "Queue default owner assignment changed.",
            "Track ownership strategy changes for queue intake."),
        new(
            "SlaActivationChanged",
            DomainOperationalEventCategory.Business,
            "SLA active/inactive status changed.",
            "Track SLA governance status changes."),
        new(
            "CustomerMergeCompensationTriggered",
            DomainOperationalEventCategory.Technical,
            "Customer merge compensation triggered after partial execution failure.",
            "Track rollback/remediation frequency for merge workflows.")
    ];

    public static DomainOperationalEventCategory Classify(string eventName)
        => Get(eventName).Category;

    public static bool TryClassify(string eventName, out DomainOperationalEventCategory category)
    {
        var match = All.FirstOrDefault(x => string.Equals(x.Name, eventName, StringComparison.Ordinal));
        if (match is null)
        {
            category = default;
            return false;
        }

        category = match.Category;
        return true;
    }

    public static DomainOperationalEventDefinition Get(string eventName)
        => All.Single(x => string.Equals(x.Name, eventName, StringComparison.Ordinal));
}
