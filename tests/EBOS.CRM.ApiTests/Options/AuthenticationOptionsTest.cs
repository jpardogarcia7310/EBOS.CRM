using EBOS.CRM.Api.Options;

namespace EBOS.CRM.ApiTests.Options;

public class AuthenticationOptionsTest
{
    [Fact]
    public void Defaults_AreExpected()
    {
        var options = new AuthenticationOptions();

        Assert.True(options.Enabled);
        Assert.False(options.UseAuthority);
        Assert.True(options.RequireHttpsMetadata);
        Assert.True(options.ValidateIssuer);
        Assert.True(options.ValidateAudience);
        Assert.Equal("sub", options.NameClaimType);
        Assert.Equal("roles", options.RoleClaimType);
        Assert.Equal(60, options.ClockSkewSeconds);
    }
}
