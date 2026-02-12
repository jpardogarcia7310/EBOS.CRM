using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Entities.EBOS;

namespace EBOS.CRM.ApiTests.Fixtures.EntitiesFactories.EBOS;

public class TenantQuotaEntityFactoryTest
{
    private static TenantQuota CreateValidTenantQuota(string metric = "ApiCalls", decimal limit = 1000m,
        string? unit = "requests", DateTime? effectiveFrom = null)
    {
        return new TenantQuota
        {
            TenantId = 1,
            Metric = metric,
            Limit = limit,
            Unit = unit,
            EffectiveFrom = effectiveFrom ?? DateTime.UtcNow.Date
        };
    }

    [Fact]
    public void CreateValidTenantQuota_Defaults_AreSet()
    {
        var quota = CreateValidTenantQuota();

        Assert.NotNull(quota);
        Assert.Equal(1, quota.TenantId);
        Assert.Equal("ApiCalls", quota.Metric);
        Assert.Equal(1000m, quota.Limit);
        Assert.Equal("requests", quota.Unit);
    }

    [Fact]
    public void CreateValidTenantQuota_CustomValues_AreApplied()
    {
        var effectiveFrom = DateTime.UtcNow.AddDays(-1);
        var quota = CreateValidTenantQuota(metric: "Storage", limit: 500m, unit: "gb", effectiveFrom: effectiveFrom);

        Assert.Equal("Storage", quota.Metric);
        Assert.Equal(500m, quota.Limit);
        Assert.Equal("gb", quota.Unit);
        Assert.Equal(effectiveFrom, quota.EffectiveFrom);
    }
}
