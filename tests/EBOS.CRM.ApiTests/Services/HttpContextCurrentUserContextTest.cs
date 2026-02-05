using System.Security.Claims;
using EBOS.CRM.Api.Services;
using Microsoft.AspNetCore.Http;

namespace EBOS.CRM.ApiTests.Services;

public class HttpContextCurrentUserContextTest
{
    [Fact]
    public void TenantId_UsesClaim_WhenPresent()
    {
        var context = new DefaultHttpContext();
        context.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim("tenantId", "5")
        }));
        context.Request.Headers["X-Tenant-Id"] = "7";

        var accessor = new HttpContextAccessor { HttpContext = context };
        var currentUser = new HttpContextCurrentUserContext(accessor);

        Assert.Equal(5, currentUser.TenantId);
    }

    [Fact]
    public void TenantId_UsesHeader_WhenClaimMissing()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers["X-Tenant-Id"] = "7";

        var accessor = new HttpContextAccessor { HttpContext = context };
        var currentUser = new HttpContextCurrentUserContext(accessor);

        Assert.Equal(7, currentUser.TenantId);
    }

    [Fact]
    public void TenantId_ReturnsZero_WhenMissing()
    {
        var context = new DefaultHttpContext();
        var accessor = new HttpContextAccessor { HttpContext = context };
        var currentUser = new HttpContextCurrentUserContext(accessor);

        Assert.Equal(0, currentUser.TenantId);
    }

    [Fact]
    public void TenantId_ReturnsZero_WhenHeaderInvalid()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers["X-Tenant-Id"] = "not-a-number";

        var accessor = new HttpContextAccessor { HttpContext = context };
        var currentUser = new HttpContextCurrentUserContext(accessor);

        Assert.Equal(0, currentUser.TenantId);
    }

    [Fact]
    public void TenantId_ReturnsZero_WhenClaimInvalid_EvenIfHeaderValid()
    {
        var context = new DefaultHttpContext();
        context.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim("tenantId", "invalid")
        }));
        context.Request.Headers["X-Tenant-Id"] = "7";

        var accessor = new HttpContextAccessor { HttpContext = context };
        var currentUser = new HttpContextCurrentUserContext(accessor);

        Assert.Equal(0, currentUser.TenantId);
    }
}
