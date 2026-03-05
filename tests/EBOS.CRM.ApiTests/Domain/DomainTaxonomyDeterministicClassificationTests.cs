using EBOS.CRM.Domain.Exceptions;

namespace EBOS.CRM.ApiTests.Domain;

public class DomainTaxonomyDeterministicClassificationTests
{
    [Theory]
    [InlineData("validation", DomainErrorTaxonomyType.DomainValidation, "DOMAIN_VALIDATION", false)]
    [InlineData("conflict", DomainErrorTaxonomyType.DomainConflict, "DOMAIN_CONFLICT", false)]
    [InlineData("rule", DomainErrorTaxonomyType.DomainRuleViolation, "DOMAIN_RULE_VIOLATION", false)]
    [InlineData("transient", DomainErrorTaxonomyType.TransientDomainFailure, "TRANSIENT_DOMAIN_FAILURE", true)]
    public void EachTaxonomyType_HasDeterministicClassification(
        string scenario,
        DomainErrorTaxonomyType expectedType,
        string expectedCode,
        bool expectedRetryable)
    {
        DomainException ex = scenario switch
        {
            "validation" => new DomainValidationException("invalid"),
            "conflict" => new DomainConflictException("conflict"),
            "rule" => new DomainRuleViolationException("rule-broken"),
            "transient" => new TransientDomainFailureException("temporary"),
            _ => throw new InvalidOperationException("Unsupported scenario.")
        };

        Assert.Equal(expectedType, ex.TaxonomyType);
        Assert.Equal(expectedCode, ex.Code);
        Assert.Equal(expectedRetryable, ex.Retryable);
    }
}
