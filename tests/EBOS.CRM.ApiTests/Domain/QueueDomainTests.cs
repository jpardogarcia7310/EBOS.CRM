using EBOS.CRM.Domain.Entities.CRM;
using FluentAssertions;

namespace EBOS.CRM.ApiTests.Domain;

public class QueueDomainTests
{
    [Fact]
    public void Activate_Sets_IsActive_True()
    {
        var entity = new Queue { IsActive = false };

        entity.Activate();

        entity.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Deactivate_Sets_IsActive_False()
    {
        var entity = new Queue { IsActive = true };

        entity.Deactivate();

        entity.IsActive.Should().BeFalse();
    }
}
