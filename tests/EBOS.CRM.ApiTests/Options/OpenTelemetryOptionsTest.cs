using EBOS.CRM.Api.Options;

namespace EBOS.CRM.ApiTests.Options;

public class OpenTelemetryOptionsTest
{
    [Fact]
    public void Defaults_AreExpected()
    {
        var o = new OpenTelemetryOptions();
        Assert.False(o.Enabled);
        Assert.Equal("ebos-crm-api", o.ServiceName);
        Assert.Equal("1.0.0", o.ServiceVersion);
        Assert.Equal("OpenTelemetry", OpenTelemetryOptions.SectionName);
    }
}
