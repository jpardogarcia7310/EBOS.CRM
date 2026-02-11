using EBOS.CRM.Domain.Entities.CRM;
using FluentAssertions;

namespace EBOS.CRM.ApiTests.Domain;

public class CaseDomainTests
{
    [Fact]
    public void SetStatus_Allows_Valid_Transition()
    {
        var entity = BuildCase(Case.StatusOpen);

        entity.SetStatus(Case.StatusInProgress);

        entity.Status.Should().Be(Case.StatusInProgress);
    }

    [Fact]
    public void SetStatus_Throws_On_Invalid_Transition()
    {
        var entity = BuildCase(Case.StatusOpen);

        var act = () => entity.SetStatus(Case.StatusReopened);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Status transition is not allowed.");
    }

    [Fact]
    public void Close_Sets_Status_And_ClosedAt()
    {
        var entity = BuildCase(Case.StatusResolved);
        var closedAt = DateTime.UtcNow;

        entity.Close(closedAt);

        entity.Status.Should().Be(Case.StatusClosed);
        entity.ClosedAt.Should().Be(closedAt);
    }

    [Fact]
    public void Reopen_Throws_When_Not_Closed()
    {
        var entity = BuildCase(Case.StatusOpen);

        var act = () => entity.Reopen();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Case is not closed.");
    }

    [Fact]
    public void AssignQueue_Throws_When_Id_Invalid()
    {
        var entity = BuildCase(Case.StatusOpen);

        var act = () => entity.AssignQueue(0);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("QueueId must be a positive value.");
    }

    private static Case BuildCase(string status) => new()
    {
        TenantId = 1,
        Title = "Case",
        Status = status,
        Priority = Case.PriorityLow,
        OwnerUserId = 1,
        QueueId = 1,
        SlaId = 1,
        CreatedAt = DateTime.UtcNow
    };
}
