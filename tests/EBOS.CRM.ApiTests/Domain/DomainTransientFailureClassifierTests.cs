using EBOS.CRM.Domain.Exceptions;

namespace EBOS.CRM.ApiTests.Domain;

public class DomainTransientFailureClassifierTests
{
    [Fact]
    public void TryClassify_TimeoutException_ReturnsDeterministicTransientCode()
    {
        var ex = new TimeoutException("db timeout");

        var classified = DomainTransientFailureClassifier.TryClassify(ex, "TestOp", out var transient);

        Assert.True(classified);
        Assert.Equal("DOMAIN_TRANSIENT_TIMEOUT", transient.Code);
        Assert.True(transient.Retryable);
    }

    [Fact]
    public void TryClassify_DomainException_DoesNotClassifyAsTransient()
    {
        var ex = new DomainValidationException("bad input");

        var classified = DomainTransientFailureClassifier.TryClassify(ex, "TestOp", out _);

        Assert.False(classified);
    }
}
