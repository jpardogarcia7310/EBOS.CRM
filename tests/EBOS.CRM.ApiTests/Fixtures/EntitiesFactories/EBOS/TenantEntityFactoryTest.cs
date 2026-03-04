using EBOS.CRM.Domain.Entities;
using EBOS.CRM.Domain.Entities.EBOS;

namespace EBOS.CRM.ApiTests.Fixtures.EntitiesFactories.EBOS;

public class TenantEntityFactoryTest
{
    private static Tenant CreateValidTenant(string code = "TEN-001", string name = "Default Tenant", bool isActive = true)
    {
        return new Tenant
        {
            Code = code,
            Name = name,
            IsActive = isActive,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = 1
        };
    }

    [Fact]
    public void CreateValidTenant_Defaults_AreSet()
    {
        var tenant = CreateValidTenant();

        Assert.NotNull(tenant);
        Assert.Equal("TEN-001", tenant.Code);
        Assert.Equal("Default Tenant", tenant.Name);
        Assert.True(tenant.IsActive);
    }

    [Fact]
    public void CreateValidTenant_CustomValues_AreApplied()
    {
        var tenant = CreateValidTenant(code: "TEN-002", name: "Acme Tenant", isActive: false);

        Assert.Equal("TEN-002", tenant.Code);
        Assert.Equal("Acme Tenant", tenant.Name);
        Assert.False(tenant.IsActive);
    }
}
