using EBOS.CRM.Domain.Entities.CRM;

namespace EBOS.CRM.ApiTests.Fixtures.EntitiesFactories.EBOS;

public class TenantUsageMetricEntityFactoryTest
{
    private static TenantUsageMetric CreateValidTenantUsageMetric(string metric = "ApiCalls", decimal value = 50m,
        string? unit = "requests", DateTime? periodStart = null, DateTime? periodEnd = null)
    {
        var start = periodStart ?? DateTime.UtcNow.Date.AddDays(-1);
        var end = periodEnd ?? DateTime.UtcNow.Date;

        return new TenantUsageMetric
        {
            TenantId = 1,
            Metric = metric,
            Value = value,
            Unit = unit,
            PeriodStart = start,
            PeriodEnd = end,
            Source = "system"
        };
    }

    [Fact]
    public void CreateValidTenantUsageMetric_Defaults_AreSet()
    {
        var metric = CreateValidTenantUsageMetric();

        Assert.NotNull(metric);
        Assert.Equal(1, metric.TenantId);
        Assert.Equal("ApiCalls", metric.Metric);
        Assert.Equal(50m, metric.Value);
        Assert.Equal("requests", metric.Unit);
        Assert.Equal("system", metric.Source);
    }

    [Fact]
    public void CreateValidTenantUsageMetric_CustomValues_AreApplied()
    {
        var start = DateTime.UtcNow.AddDays(-10);
        var end = DateTime.UtcNow.AddDays(-5);
        var metric = CreateValidTenantUsageMetric(metric: "Storage", value: 3.5m, unit: "gb",
            periodStart: start, periodEnd: end);

        Assert.Equal("Storage", metric.Metric);
        Assert.Equal(3.5m, metric.Value);
        Assert.Equal("gb", metric.Unit);
        Assert.Equal(start, metric.PeriodStart);
        Assert.Equal(end, metric.PeriodEnd);
    }
}
