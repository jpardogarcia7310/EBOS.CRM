using EBOS.CRM.Domain.Exceptions;

namespace EBOS.CRM.ApiTests.Domain;

public class DomainErrorTaxonomyCatalogTests
{
    [Fact]
    public void Taxonomy_Concept_IsClassification_NotEntity()
    {
        Assert.False(DomainErrorTaxonomy.RepresentsDomainEntities);
        Assert.Contains("classification model", DomainErrorTaxonomy.Concept, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData(DomainErrorTaxonomyType.DomainValidation, "DOMAIN_VALIDATION", false)]
    [InlineData(DomainErrorTaxonomyType.DomainConflict, "DOMAIN_CONFLICT", false)]
    [InlineData(DomainErrorTaxonomyType.DomainRuleViolation, "DOMAIN_RULE_VIOLATION", false)]
    [InlineData(DomainErrorTaxonomyType.TransientDomainFailure, "TRANSIENT_DOMAIN_FAILURE", true)]
    public void Taxonomy_Definitions_AreComplete_AndDeterministic(
        DomainErrorTaxonomyType type,
        string canonicalCode,
        bool defaultRetryable)
    {
        var def = DomainErrorTaxonomy.Get(type);

        Assert.Equal(type, def.Type);
        Assert.Equal(canonicalCode, def.CanonicalCode);
        Assert.Equal(defaultRetryable, def.DefaultRetryable);
        Assert.False(string.IsNullOrWhiteSpace(def.Definition));
        Assert.False(string.IsNullOrWhiteSpace(def.Usage));
        Assert.Contains("not a persisted domain entity", def.NotEntityExplanation, StringComparison.OrdinalIgnoreCase);
    }
}
