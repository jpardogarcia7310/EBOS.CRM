using System.Security.Claims;
using EBOS.CRM.Api.Services;
using EBOS.CRM.Api.Constants;
using Microsoft.AspNetCore.Http;

namespace EBOS.CRM.ApiTests.Services;

public class HttpContextCurrentUserContextTest
{
    [Fact]
    public void TenantId_UsesClaim_WhenPresent()
    {
        var context = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim("tenantId", "5")
            }))
        };
        context.Request.Headers[HeaderNames.TenantId] = "7";

        var accessor = new HttpContextAccessor { HttpContext = context };
        var currentUser = new HttpContextCurrentUserContext(accessor);

        Assert.Equal(5, currentUser.TenantId);
    }

    [Fact]
    public void TenantId_UsesHeader_WhenClaimMissing()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers[HeaderNames.TenantId] = "7";

        var accessor = new HttpContextAccessor { HttpContext = context };
        var currentUser = new HttpContextCurrentUserContext(accessor);

        Assert.Equal(7, currentUser.TenantId);
    }

    [Fact]
    public void TenantId_UsesItem_WhenHeaderAndClaimMissing()
    {
        var context = new DefaultHttpContext();
        context.Items[TenantContextKeys.TenantId] = 9L;

        var accessor = new HttpContextAccessor { HttpContext = context };
        var currentUser = new HttpContextCurrentUserContext(accessor);

        Assert.Equal(9, currentUser.TenantId);
    }

    [Fact]
    public void TenantId_UsesClaim_WhenItemPresent()
    {
        var context = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim("tenantId", "5")
            }))
        };
        context.Items[TenantContextKeys.TenantId] = 9L;

        var accessor = new HttpContextAccessor { HttpContext = context };
        var currentUser = new HttpContextCurrentUserContext(accessor);

        Assert.Equal(5, currentUser.TenantId);
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
        context.Request.Headers[HeaderNames.TenantId] = "not-a-number";

        var accessor = new HttpContextAccessor { HttpContext = context };
        var currentUser = new HttpContextCurrentUserContext(accessor);

        Assert.Equal(0, currentUser.TenantId);
    }

    [Fact]
    public void TenantId_UsesItem_WhenHeaderInvalid()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers[HeaderNames.TenantId] = "not-a-number";
        context.Items[TenantContextKeys.TenantId] = 4L;

        var accessor = new HttpContextAccessor { HttpContext = context };
        var currentUser = new HttpContextCurrentUserContext(accessor);

        Assert.Equal(4, currentUser.TenantId);
    }

    [Fact]
    public void TenantId_ReturnsZero_WhenClaimInvalid_EvenIfHeaderValid()
    {
        var context = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim("tenantId", "invalid")
            }))
        };
        context.Request.Headers[HeaderNames.TenantId] = "7";

        var accessor = new HttpContextAccessor { HttpContext = context };
        var currentUser = new HttpContextCurrentUserContext(accessor);

        Assert.Equal(0, currentUser.TenantId);
    }

    [Fact]
    public void TenantId_UsesTenantIdClaim_WhenTenantIdAndTenant_idPresent()
    {
        var context = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim("tenantId", "3"),
                new Claim("tenant_id", "9")
            }))
        };

        var accessor = new HttpContextAccessor { HttpContext = context };
        var currentUser = new HttpContextCurrentUserContext(accessor);

        Assert.Equal(3, currentUser.TenantId);
    }

    [Fact]
    public void UserId_UsesSub_WhenPresent()
    {
        var context = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim("sub", "11"),
                new Claim("userId", "22"),
                new Claim(ClaimTypes.NameIdentifier, "33")
            }))
        };

        var accessor = new HttpContextAccessor { HttpContext = context };
        var currentUser = new HttpContextCurrentUserContext(accessor);

        Assert.Equal(11, currentUser.UserId);
    }

    [Fact]
    public void UserId_UsesUserId_WhenSubMissing()
    {
        var context = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim("userId", "22"),
                new Claim(ClaimTypes.NameIdentifier, "33")
            }))
        };

        var accessor = new HttpContextAccessor { HttpContext = context };
        var currentUser = new HttpContextCurrentUserContext(accessor);

        Assert.Equal(22, currentUser.UserId);
    }

    [Fact]
    public void UserId_UsesNameIdentifier_WhenOthersMissing()
    {
        var context = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "33")
            }))
        };

        var accessor = new HttpContextAccessor { HttpContext = context };
        var currentUser = new HttpContextCurrentUserContext(accessor);

        Assert.Equal(33, currentUser.UserId);
    }

    [Fact]
    public void UserId_ReturnsZero_WhenInvalid()
    {
        var context = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim("sub", "invalid")
            }))
        };

        var accessor = new HttpContextAccessor { HttpContext = context };
        var currentUser = new HttpContextCurrentUserContext(accessor);

        Assert.Equal(0, currentUser.UserId);
    }

    [Fact]
    public void UserId_ReturnsZero_WhenMissing()
    {
        var context = new DefaultHttpContext();
        var accessor = new HttpContextAccessor { HttpContext = context };
        var currentUser = new HttpContextCurrentUserContext(accessor);

        Assert.Equal(0, currentUser.UserId);
    }
}
