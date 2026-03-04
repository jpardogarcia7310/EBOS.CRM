using EBOS.CRM.Domain.Entities.CRM;
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
    }

    [Fact]
    public void SetStatus_Throws_On_Invalid_Status()
    {
        var activity = BuildActivity(CaseActivity.StatusOpen);

        var act = () => activity.SetStatus("BadStatus");

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Status value is invalid.");
    }

    [Fact]
    public void SetStatus_Throws_On_Invalid_Transition()
    {
        var activity = BuildActivity(CaseActivity.StatusOpen);

        var act = () => activity.SetStatus(CaseActivity.StatusCancelled);
        act.Should().NotThrow();

        var actAfter = () => activity.SetStatus(CaseActivity.StatusInProgress);
        actAfter.Should().Throw<InvalidOperationException>()
            .WithMessage("Status transition is not allowed.");
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
