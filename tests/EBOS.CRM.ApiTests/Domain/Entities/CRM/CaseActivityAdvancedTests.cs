using EBOS.CRM.Domain.Entities.CRM;

namespace EBOS.CRM.ApiTests.Domain.Entities.CRM;

public class CaseActivityAdvancedTests
{
    [Fact]
    public void SetStatus_FromEmpty_AllowsOnlyOpen()
    {
        var activity = BuildActivity(string.Empty);

        Assert.Throws<InvalidOperationException>(() => activity.SetStatus(CaseActivity.StatusInProgress));

        activity.SetStatus(CaseActivity.StatusOpen);
        Assert.Equal(CaseActivity.StatusOpen, activity.Status);
    }

    [Fact]
    public void SetStatus_FromCompleted_RejectsAnyFurtherTransition()
    {
        var activity = BuildActivity(CaseActivity.StatusCompleted);
        Assert.Throws<InvalidOperationException>(() => activity.SetStatus(CaseActivity.StatusCancelled));
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
