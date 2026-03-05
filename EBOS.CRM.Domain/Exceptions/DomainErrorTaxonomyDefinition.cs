namespace EBOS.CRM.Domain.Exceptions;

public sealed record DomainErrorTaxonomyDefinition(
    DomainErrorTaxonomyType Type,
    string CanonicalCode,
    bool DefaultRetryable,
    string Definition,
    string Usage,
    string NotEntityExplanation);
