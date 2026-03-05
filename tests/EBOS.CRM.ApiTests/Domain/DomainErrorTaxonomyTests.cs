using EBOS.CRM.Domain.Exceptions;

namespace EBOS.CRM.ApiTests.Domain;

public class DomainErrorTaxonomyTests
{
    [Fact]
    public void DomainValidation_Defaults_AreDeterministic()
    {
        var ex = new DomainValidationException("invalid value");

        Assert.Equal(DomainErrorTaxonomyType.DomainValidation, ex.TaxonomyType);
        Assert.Equal("DOMAIN_VALIDATION", ex.Code);
        Assert.False(ex.Retryable);
    }

    [Fact]
    public void DomainConflict_Defaults_AreDeterministic()
    {
        var ex = new DomainConflictException("state collision");

        Assert.Equal(DomainErrorTaxonomyType.DomainConflict, ex.TaxonomyType);
        Assert.Equal("DOMAIN_CONFLICT", ex.Code);
        Assert.False(ex.Retryable);
    }

    [Fact]
    public void DomainRuleViolation_Defaults_AreDeterministic()
    {
        var ex = new DomainRuleViolationException("business invariant broken");

        Assert.Equal(DomainErrorTaxonomyType.DomainRuleViolation, ex.TaxonomyType);
        Assert.Equal("DOMAIN_RULE_VIOLATION", ex.Code);
        Assert.False(ex.Retryable);
    }

    [Fact]
    public void TransientDomainFailure_Defaults_AreDeterministic()
    {
        var ex = new TransientDomainFailureException("temporary domain infrastructure issue");

        Assert.Equal(DomainErrorTaxonomyType.TransientDomainFailure, ex.TaxonomyType);
        Assert.Equal("TRANSIENT_DOMAIN_FAILURE", ex.Code);
        Assert.True(ex.Retryable);
    }
}
