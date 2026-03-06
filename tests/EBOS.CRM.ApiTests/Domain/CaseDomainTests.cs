using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Events;
using EBOS.CRM.Domain.Exceptions;
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
        entity.PeekOperationalEvents().Should().Contain(x =>
            x.Name == "CaseStatusChanged" &&
            x.Category == DomainOperationalEventCategory.Business);
    }

    [Fact]
    public void SetStatus_Throws_On_Invalid_Transition()
    {
        var entity = BuildCase(Case.StatusOpen);

        var act = () => entity.SetStatus(Case.StatusReopened);

        act.Should().Throw<DomainRuleViolationException>()
            .WithMessage("Status transition is not allowed.");
        entity.PeekOperationalEvents().Should().Contain(x =>
            x.Name == "DomainInvariantBreachDetected" &&
            x.Category == DomainOperationalEventCategory.Anomaly);
    }

    [Fact]
    public void SetStatus_SameStatus_IsIdempotentAndEmitsTechnicalDedupEvent()
    {
        var entity = BuildCase(Case.StatusOpen);

        entity.SetStatus(Case.StatusOpen);

        entity.Status.Should().Be(Case.StatusOpen);
        entity.PeekOperationalEvents().Should().Contain(x =>
            x.Name == "DomainCommandDeduplicated" &&
            x.Category == DomainOperationalEventCategory.Technical);
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

        act.Should().Throw<DomainRuleViolationException>()
            .WithMessage("Case is not closed.");
    }

    [Fact]
    public void AssignQueue_Throws_When_Id_Invalid()
    {
        var entity = BuildCase(Case.StatusOpen);

        var act = () => entity.AssignQueue(0);

        act.Should().Throw<DomainValidationException>()
            .WithMessage("QueueId must be a positive value.");
    }

    [Fact]
    public void Open_Sets_Status_When_Uninitialized()
    {
        var entity = BuildCase(string.Empty);

        entity.Open();

        entity.Status.Should().Be(Case.StatusOpen);
    }

    [Fact]
    public void Open_Throws_When_Already_Initialized()
    {
        var entity = BuildCase(Case.StatusOpen);

        var act = () => entity.Open();

        act.Should().Throw<DomainRuleViolationException>()
            .WithMessage("Case is already initialized.");
    }

    [Fact]
    public void UpdateDetails_Updates_Title_And_Description()
    {
        var entity = BuildCase(Case.StatusOpen);

        entity.UpdateDetails("New Title", "Updated");

        entity.Title.Should().Be("New Title");
        entity.Description.Should().Be("Updated");
    }

    [Fact]
    public void UpdateDetails_Throws_When_Title_Empty()
    {
        var entity = BuildCase(Case.StatusOpen);

        var act = () => entity.UpdateDetails(" ", "Updated");

        act.Should().Throw<DomainValidationException>()
            .WithMessage("Title is required.");
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
