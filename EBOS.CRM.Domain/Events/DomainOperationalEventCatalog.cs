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
            "CustomerPrivacyRequestCompensationTriggered",
            DomainOperationalEventCategory.Technical,
            "A failed privacy request was explicitly compensated back to pending for retry.",
            "Track compensation rate and validate reversible-domain behavior."),
        new(
            "CustomerPrivacyRequestCompleted",
            DomainOperationalEventCategory.Business,
            "A privacy request completed successfully.",
            "Track business completion outcomes for privacy workflows.")
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
