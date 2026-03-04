using EBOS.CRM.Api.Options;

namespace EBOS.CRM.ApiTests.Options;

public class OidcAndPrivacyRetentionOptionsTest
{
    [Fact]
    public void OidcOptions_Defaults_AreExpected()
    {
        var options = new OidcOptions();

        Assert.True(options.RequireHttpsMetadata);
        Assert.Equal(60, options.ClockSkewSeconds);
        Assert.Equal(30, options.BackchannelTimeoutSeconds);
        Assert.Equal("roles", options.RoleClaimType);
        Assert.Equal("permissions", options.PermissionClaimType);
    }

    [Fact]
    public void CustomerPrivacyRetentionJobOptions_Defaults_AreExpected()
    {
        var options = new CustomerPrivacyRetentionJobOptions();

        Assert.False(options.Enabled);
        Assert.Equal(60, options.SweepIntervalMinutes);
        Assert.True(options.DryRun);
        Assert.Equal(500, options.BatchSize);
        Assert.Equal(1, options.SystemUserId);
    }
}
