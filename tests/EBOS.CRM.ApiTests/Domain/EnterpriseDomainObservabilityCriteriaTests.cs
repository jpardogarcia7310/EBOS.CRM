using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Events;
using EBOS.CRM.Domain.Exceptions;

namespace EBOS.CRM.ApiTests.Domain;

public class EnterpriseDomainObservabilityCriteriaTests
{
    [Fact]
    public void CompensatingAction_PreservesInvariants_AfterPartialFailure()
    {
        var request = CustomerPrivacyRequest.Create(
            tenantId: 1,
            customerId: 100,
            requestType: CustomerPrivacyRequest.TypeAnonymize,
            requestedBy: 7,
            reason: "gdpr",
            correlationId: "corr-1");

        request.MarkInProgress(9);
        request.MarkFailed(9, "TIMEOUT", "partial execution");

        request.CompensateToPendingForRetry(9, "retry approved");

        Assert.Equal(CustomerPrivacyRequest.StatusPending, request.Status);
        Assert.Null(request.FailureCode);
        Assert.Null(request.FailureReason);

        request.MarkInProgress(9);
        request.MarkCompleted(9);

        Assert.Equal(CustomerPrivacyRequest.StatusCompleted, request.Status);
        Assert.Null(request.FailureCode);
        Assert.Null(request.FailureReason);
    }

    [Fact]
    public void LongRunningWorkflow_AllowsOnlyMonotonicTransitions()
    {
        var request = CustomerPrivacyRequest.Create(
            tenantId: 1,
            customerId: 101,
            requestType: CustomerPrivacyRequest.TypeForget,
            requestedBy: 7,
            reason: "privacy",
            correlationId: "corr-2");

        request.MarkInProgress(11);
        request.MarkFailed(11, "EXECUTION_ERROR", "step-1");
        request.CompensateToPendingForRetry(11, "step-2");
        request.MarkInProgress(11);
        request.MarkCompleted(11);

        var ex = Assert.Throws<DomainRuleViolationException>(() => request.MarkInProgress(11));
        Assert.Equal("DOMAIN_RULE_VIOLATION_PRIVACY_REQUEST_TRANSITION_IN_PROGRESS", ex.Code);
    }

    [Fact]
    public void EmittedOperationalEvents_HaveConsistentDeterministicClassification_ForAnalyticsConsumers()
    {
        var request = CustomerPrivacyRequest.Create(
            tenantId: 1,
            customerId: 102,
            requestType: CustomerPrivacyRequest.TypeAnonymize,
            requestedBy: 7,
            reason: "privacy",
            correlationId: "corr-3");

        request.MarkInProgress(11);
        request.MarkInProgress(11); // dedup technical event
        request.MarkFailed(11, "EXECUTION_ERROR", "failed");
        request.CompensateToPendingForRetry(11, "retry");
        request.MarkInProgress(11);
        request.MarkCompleted(11);

        var emitted = request.DequeueOperationalEvents();

        Assert.NotEmpty(emitted);
        Assert.All(emitted, evt =>
        {
            var fromCatalog = DomainOperationalEventCatalog.Get(evt.Name);
            Assert.Equal(fromCatalog.Category, evt.Category);
            Assert.Equal(DomainOperationalEventCatalog.Classify(evt.Name), evt.Category);
        });
    }
}
