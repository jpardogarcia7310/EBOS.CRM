using EBOS.CRM.Domain.Entities.CRM;
using FluentAssertions;

namespace EBOS.CRM.ApiTests.Domain;

public class SlaDomainTests
{
    [Fact]
    public void CalculateDueAt_Adds_TargetMinutes()
    {
        var entity = BuildSla(30);
        var start = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc);

        var dueAt = entity.CalculateDueAt(start);

        dueAt.Should().Be(start.AddMinutes(30));
    }

    [Fact]
    public void CalculateDueAt_Throws_When_TargetMinutes_Invalid()
    {
        var entity = BuildSla(0);

        var act = () => entity.CalculateDueAt(DateTime.UtcNow);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("TargetMinutes must be greater than zero.");
    }

    [Fact]
    public void ValidateWarningMinutes_Throws_When_Too_High()
    {
        var entity = BuildSla(30);
        entity.WarningMinutes = 40;

        var act = () => entity.ValidateWarningMinutes();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("WarningMinutes cannot exceed TargetMinutes.");
    }

    [Fact]
    public void ValidateActiveRange_Throws_When_From_After_To()
    {
        var entity = BuildSla(30);
        entity.ActiveFrom = DateTime.UtcNow.AddDays(2);
        entity.ActiveTo = DateTime.UtcNow.AddDays(1);

        var act = () => entity.ValidateActiveRange();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("ActiveFrom cannot be later than ActiveTo.");
    }

    [Fact]
    public void IsActiveAt_Returns_False_When_Not_Active()
    {
        var entity = BuildSla(30);
        entity.IsActive = false;

        entity.IsActiveAt(DateTime.UtcNow).Should().BeFalse();
    }

    [Fact]
    public void IsBreached_Returns_True_When_DueAt_Passed()
    {
        var entity = BuildSla(30);
        var now = DateTime.UtcNow;

        entity.IsBreached(now, now.AddMinutes(-1)).Should().BeTrue();
    }

    private static Sla BuildSla(int targetMinutes) => new()
    {
        TenantId = 1,
        Name = "SLA",
        TargetMinutes = targetMinutes,
        WarningMinutes = 5,
        IsActive = true
    };
}
