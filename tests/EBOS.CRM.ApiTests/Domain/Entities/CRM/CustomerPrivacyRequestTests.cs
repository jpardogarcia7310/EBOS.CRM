using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Exceptions;

namespace EBOS.CRM.ApiTests.Domain.Entities.CRM;

public class CustomerPrivacyRequestTests
{
    [Fact]
    public void Create_InvalidRequestType_Throws()
    {
        Assert.ThrowsAny<DomainException>(() =>
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
        Assert.ThrowsAny<DomainException>(() => entity.MarkCompleted(10));
    }

    [Fact]
    public void MarkFailed_WithoutInProgress_Throws()
    {
        var entity = CustomerPrivacyRequest.Create(1, 2, CustomerPrivacyRequest.TypeForget, 3, null, null);
        Assert.ThrowsAny<DomainException>(() => entity.MarkFailed(10, "ERR", "fail"));
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

    [Fact]
    public void CompensationAction_FailedToPending_IsExplicitAndDeterministic()
    {
        var entity = CustomerPrivacyRequest.Create(1, 2, CustomerPrivacyRequest.TypeAnonymize, 3, null, null);
        entity.MarkInProgress(10);
        entity.MarkFailed(10, "EXECUTION_ERROR", "boom");

        entity.CompensateToPendingForRetry(20, "retry-compensation");

        Assert.Equal(CustomerPrivacyRequest.StatusPending, entity.Status);
        Assert.Null(entity.FailureCode);
        Assert.Null(entity.FailureReason);
        Assert.Equal("retry-compensation", entity.Reason);
    }

    [Fact]
    public void DuplicateBusinessActionUnderRetry_DoesNotDuplicateStateMutation()
    {
        var entity = CustomerPrivacyRequest.Create(1, 2, CustomerPrivacyRequest.TypeAnonymize, 3, null, null);

        entity.MarkInProgress(10);
        var firstProcessedAt = entity.ProcessedAt;
        entity.MarkInProgress(10);
        var secondProcessedAt = entity.ProcessedAt;

        Assert.Equal(CustomerPrivacyRequest.StatusInProgress, entity.Status);
        Assert.Equal(firstProcessedAt, secondProcessedAt);
    }

    [Fact]
    public void MonotonicWorkflowInvariant_DisallowsIllegalJumpFromPendingToCompleted()
    {
        var entity = CustomerPrivacyRequest.Create(1, 2, CustomerPrivacyRequest.TypeForget, 3, null, null);

        var ex = Assert.ThrowsAny<DomainException>(() => entity.MarkCompleted(10));
        Assert.IsType<DomainRuleViolationException>(ex);
    }
}


