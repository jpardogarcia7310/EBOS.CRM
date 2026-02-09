using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;
using EBOS.CRM.Infrastructure.Services.TenantInvariants;
using FluentAssertions;

namespace EBOS.CRM.ApiTests.Domain;

public class TenantInvariantsTests
{
    [Fact]
    public void EnsureTenantAssigned_Throws_When_TenantId_Missing()
    {
        var entity = new TestTenantEntity { TenantId = 0 };

        var act = () => TenantInvariants.EnsureTenantAssigned(entity);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("TenantId is required.");
    }

    [Fact]
    public void EnsureTenantAssigned_Allows_When_TenantId_Present()
    {
        var entity = new TestTenantEntity { TenantId = 5 };

        TenantInvariants.EnsureTenantAssigned(entity);
    }

    private sealed class TestTenantEntity : ITenantScopedEntity
    {
        public long TenantId { get; set; }
    }
}
