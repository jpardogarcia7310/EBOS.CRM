using EBOS.CRM.Domain.Entities.CRM;

namespace EBOS.CRM.ApiTests.Domain.Entities.CRM;

public class CustomerPrivacyRequestTests
{
    [Fact]
    public void Create_InvalidRequestType_Throws()
    {
        Assert.Throws<InvalidOperationException>(() =>
            CustomerPrivacyRequest.Create(1, 2, "UNKNOWN", 3, null, null));
    }

    [Fact]
    public void Create_NormalizesTypeAndOptionalFields()
    {
        var entity = CustomerPrivacyRequest.Create(1, 2, " anonymize ", 3, " reason ", " corr ");

        Assert.Equal(CustomerPrivacyRequest.TypeAnonymize, entity.RequestType);
        Assert.Equal(CustomerPrivacyRequest.StatusPending, entity.Status);
        Assert.Equal("reason", entity.Reason);
        Assert.Equal("corr", entity.CorrelationId);
    }

    [Fact]
    public void MarkCompleted_WithoutInProgress_Throws()
    {
        var entity = CustomerPrivacyRequest.Create(1, 2, CustomerPrivacyRequest.TypeForget, 3, null, null);
        Assert.Throws<InvalidOperationException>(() => entity.MarkCompleted(10));
    }

    [Fact]
    public void MarkFailed_WithoutInProgress_Throws()
    {
        var entity = CustomerPrivacyRequest.Create(1, 2, CustomerPrivacyRequest.TypeForget, 3, null, null);
        Assert.Throws<InvalidOperationException>(() => entity.MarkFailed(10, "ERR", "fail"));
    }

    [Fact]
    public void RetryFlow_FailedToPending_ClearsFailure()
    {
        var entity = CustomerPrivacyRequest.Create(1, 2, CustomerPrivacyRequest.TypeAnonymize, 3, null, null);
        entity.MarkInProgress(10);
        entity.MarkFailed(10, "ERR_CODE", "boom");

        entity.MarkPendingForRetry(20, "retry reason");

        Assert.Equal(CustomerPrivacyRequest.StatusPending, entity.Status);
        Assert.Null(entity.FailureCode);
        Assert.Null(entity.FailureReason);
        Assert.Equal("retry reason", entity.Reason);
        Assert.Equal(20, entity.ProcessedBy);
    }
}
