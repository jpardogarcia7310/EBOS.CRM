namespace EBOS.CRM.Domain.Exceptions;

public static class DomainErrorTaxonomy
{
    public const string Concept =
        "A taxonomy is a classification model for domain failures used to standardize behavior, logging, and API mapping.";

    public const bool RepresentsDomainEntities = false;

    public static IReadOnlyCollection<DomainErrorTaxonomyDefinition> All { get; } =
    [
        new(
            DomainErrorTaxonomyType.DomainValidation,
            "DOMAIN_VALIDATION",
            DefaultRetryable: false,
            Definition: "Input or aggregate state shape is invalid before business rule execution.",
            Usage: "Use for missing required values, invalid format, and out-of-range values.",
            NotEntityExplanation: "This is an error classification, not a persisted domain entity."),
        new(
            DomainErrorTaxonomyType.DomainConflict,
            "DOMAIN_CONFLICT",
            DefaultRetryable: false,
            Definition: "Requested operation collides with current domain/persisted state.",
            Usage: "Use for duplicate actions, version mismatch, or already-processed commands.",
            NotEntityExplanation: "This is an error classification, not a persisted domain entity."),
        new(
            DomainErrorTaxonomyType.DomainRuleViolation,
            "DOMAIN_RULE_VIOLATION",
            DefaultRetryable: false,
            Definition: "A business invariant is violated with otherwise syntactically valid input.",
            Usage: "Use for illegal state transitions and business rule breaches.",
            NotEntityExplanation: "This is an error classification, not a persisted domain entity."),
        new(
            DomainErrorTaxonomyType.TransientDomainFailure,
            "TRANSIENT_DOMAIN_FAILURE",
            DefaultRetryable: true,
            Definition: "Temporary domain-level execution barrier caused by short-lived conditions.",
            Usage: "Use for lock timeouts, transient stale reads, and temporary domain service unavailability.",
            NotEntityExplanation: "This is an error classification, not a persisted domain entity.")
    ];

    public static DomainErrorTaxonomyDefinition Get(DomainErrorTaxonomyType type)
        => All.Single(x => x.Type == type);
}
