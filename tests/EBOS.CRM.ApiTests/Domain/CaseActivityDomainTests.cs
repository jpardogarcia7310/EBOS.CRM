using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Events;
using EBOS.CRM.Domain.Exceptions;
using FluentAssertions;

namespace EBOS.CRM.ApiTests.Domain;

public class CaseActivityDomainTests
{
    [Fact]
    public void SetStatus_Allows_Valid_Transition()
    {
        var activity = BuildActivity(CaseActivity.StatusOpen);

        activity.SetStatus(CaseActivity.StatusInProgress);

        activity.Status.Should().Be(CaseActivity.StatusInProgress);
        activity.PeekOperationalEvents().Should().Contain(x =>
            x.Name == "CaseActivityStatusChanged" &&
            x.Category == DomainOperationalEventCategory.Business);
    }

    [Fact]
    public void SetStatus_Throws_On_Invalid_Status()
    {
        var activity = BuildActivity(CaseActivity.StatusOpen);

        var act = () => activity.SetStatus("BadStatus");

        act.Should().Throw<DomainValidationException>()
            .WithMessage("Status value is invalid.");
    }

    [Fact]
    public void SetStatus_Throws_On_Invalid_Transition()
    {
        var activity = BuildActivity(CaseActivity.StatusOpen);

        var act = () => activity.SetStatus(CaseActivity.StatusCancelled);
        act.Should().NotThrow();

        var actAfter = () => activity.SetStatus(CaseActivity.StatusInProgress);
        actAfter.Should().Throw<DomainRuleViolationException>()
            .WithMessage("Status transition is not allowed.");
        activity.PeekOperationalEvents().Should().Contain(x =>
            x.Name == "DomainInvariantBreachDetected" &&
            x.Category == DomainOperationalEventCategory.Anomaly);
    }

    [Fact]
    public void SetStatus_SameStatus_IsIdempotentAndEmitsTechnicalDedupEvent()
    {
        var activity = BuildActivity(CaseActivity.StatusOpen);

        activity.SetStatus(CaseActivity.StatusOpen);

        activity.Status.Should().Be(CaseActivity.StatusOpen);
        activity.PeekOperationalEvents().Should().Contain(x =>
            x.Name == "DomainCommandDeduplicated" &&
            x.Category == DomainOperationalEventCategory.Technical);
    }

    private static CaseActivity BuildActivity(string status) => new()
    {
        TenantId = 1,
        CaseId = 1,
        Title = "Activity",
        Status = status,
        CreatedAt = DateTime.UtcNow,
        CreatedBy = 1
    };
}
