using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Entities.EBOS;

namespace EBOS.CRM.ApiTests.Fixtures.EntitiesFactories.EBOS;

public class TenantConfigurationEntityFactoryTest
{
    private static TenantConfiguration CreateValidEntity(long tenantId = 1, string key = "limits.maxUsers",
        string valueJson = "{\"value\":25}")
    {
        return new TenantConfiguration
        {
            TenantId = tenantId,
            Key = key,
            ValueJson = valueJson,
            UpdatedAt = DateTime.UtcNow,
            UpdatedBy = 1
        };
    }

    [Fact]
    public void CreateValidTenantConfiguration_Defaults_AreSet()
    {
        var entity = CreateValidEntity();

        Assert.NotNull(entity);
        Assert.Equal(1, entity.TenantId);
        Assert.Equal("limits.maxUsers", entity.Key);
        Assert.Equal("{\"value\":25}", entity.ValueJson);
    }

    [Fact]
    public void CreateValidTenantConfiguration_CustomValues_AreApplied()
    {
        var entity = CreateValidEntity(tenantId: 2, key: "features.beta", valueJson: "{\"enabled\":false}");

        Assert.Equal(2, entity.TenantId);
        Assert.Equal("features.beta", entity.Key);
        Assert.Equal("{\"enabled\":false}", entity.ValueJson);
    }
}
