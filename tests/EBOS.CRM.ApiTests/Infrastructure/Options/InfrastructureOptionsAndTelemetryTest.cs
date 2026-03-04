using EBOS.CRM.Infrastructure.Observability;
using EBOS.CRM.Infrastructure.Options;
using EBOS.CRM.Infrastructure.Services.Audit;

namespace EBOS.CRM.ApiTests.Infrastructure.Options;

public class InfrastructureOptionsAndTelemetryTest
{
    [Fact]
    public void AuditOutboxOptions_Defaults()
    {
        var o = new AuditOutboxOptions();
        Assert.True(o.Enabled);
        Assert.Equal(10, o.MaxAttempts);
        Assert.Equal(50, o.BatchSize);
        Assert.Equal(30, o.DispatchIntervalSeconds);
        Assert.Equal("AuditOutbox", AuditOutboxOptions.SectionName);
    }

    [Fact]
    public void AuditServiceOptions_Defaults()
    {
        var o = new AuditServiceOptions();
        Assert.True(o.Enabled);
        Assert.Equal(10, o.TimeoutSeconds);
        Assert.Equal(3, o.RetryCount);
        Assert.Equal("AuditService", AuditServiceOptions.SectionName);
    }

    [Fact]
    public void CustomerDedupeOptions_Defaults()
    {
        var o = new CustomerDedupeOptions();
        Assert.Equal("CustomerDedupe", CustomerDedupeOptions.SectionName);
        Assert.True(o.EnablePhoneSuffixFallback);
        Assert.True(o.MinScore > 0);
    }

    [Fact]
    public void TelemetryNames_Constants()
    {
        Assert.Equal("EBOS.CRM.Customer360", TelemetryNames.Customer360Meter);
        Assert.Equal("EBOS.CRM.Customer360", TelemetryNames.Customer360ActivitySource);
        Assert.Equal("EBOS.CRM.Audit", TelemetryNames.AuditActivitySource);
    }
}
