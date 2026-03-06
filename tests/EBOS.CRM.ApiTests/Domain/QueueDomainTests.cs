using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Exceptions;
using FluentAssertions;

namespace EBOS.CRM.ApiTests.Domain;

public class QueueDomainTests
{
    [Fact]
    public void ToggleActive_Sets_IsActive_True()
    {
        var entity = new Queue { IsActive = false };

        entity.ToggleActive(true, hasOpenCases: false);

        entity.IsActive.Should().BeTrue();
    }

    [Fact]
    public void ToggleActive_Throws_When_Deactivating_With_OpenCases()
    {
        var entity = new Queue { IsActive = true };

        var act = () => entity.ToggleActive(false, hasOpenCases: true);

        act.Should().Throw<DomainRuleViolationException>();
    }

    [Fact]
    public void AssignDefaultOwner_Throws_When_Invalid()
    {
        var entity = new Queue();

        var act = () => entity.AssignDefaultOwner(0);

        act.Should().Throw<DomainValidationException>();
    }
}
