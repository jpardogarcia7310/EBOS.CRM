using EBOS.CRM.Api.Constants;
using EBOS.CRM.Api.Middleware;
using EBOS.CRM.Api.Options;
using Microsoft.AspNetCore.Http;
namespace EBOS.CRM.ApiTests.Middleware;

public class TenantResolutionMiddlewareTest
{
    [Fact]
    public async Task Invoke_ResolvesTenant_FromHeader()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers[HeaderNames.TenantId] = "12";
        var options = Microsoft.Extensions.Options.Options.Create(new TenantResolutionOptions
        {
            EnableHeader = true,
            EnableSubdomain = false,
            HeaderName = HeaderNames.TenantId
        });
        var middleware = new TenantResolutionMiddleware(_ => Task.CompletedTask, options);

        await middleware.Invoke(context);

        Assert.Equal(12L, context.Items[TenantContextKeys.TenantId]);
    }

    [Fact]
    public async Task Invoke_ResolvesTenant_FromSubdomain()
    {
        var context = new DefaultHttpContext();
        context.Request.Host = new HostString("tenant42.api.domain");
        var options = Microsoft.Extensions.Options.Options.Create(new TenantResolutionOptions
        {
            EnableHeader = false,
            EnableSubdomain = true,
            SubdomainPrefix = "tenant"
        });
        var middleware = new TenantResolutionMiddleware(_ => Task.CompletedTask, options);

        await middleware.Invoke(context);

        Assert.Equal(42L, context.Items[TenantContextKeys.TenantId]);
    }

    [Fact]
    public async Task Invoke_PrefersHeader_WhenHeaderAndSubdomainPresent()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers[HeaderNames.TenantId] = "7";
        context.Request.Host = new HostString("tenant9.api.domain");
        var options = Microsoft.Extensions.Options.Options.Create(new TenantResolutionOptions
        {
            EnableHeader = true,
            EnableSubdomain = true,
            HeaderName = HeaderNames.TenantId,
            SubdomainPrefix = "tenant"
        });
        var middleware = new TenantResolutionMiddleware(_ => Task.CompletedTask, options);

        await middleware.Invoke(context);

        Assert.Equal(7L, context.Items[TenantContextKeys.TenantId]);
    }

    [Fact]
    public async Task Invoke_DoesNotResolve_When_HeaderInvalid()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers[HeaderNames.TenantId] = "invalid";
        var options = Microsoft.Extensions.Options.Options.Create(new TenantResolutionOptions
        {
            EnableHeader = true,
            EnableSubdomain = false,
            HeaderName = HeaderNames.TenantId
        });
        var middleware = new TenantResolutionMiddleware(_ => Task.CompletedTask, options);

        await middleware.Invoke(context);

        Assert.False(context.Items.ContainsKey(TenantContextKeys.TenantId));
    }

    [Fact]
    public async Task Invoke_DoesNotResolve_When_SubdomainInvalid()
    {
        var context = new DefaultHttpContext();
        context.Request.Host = new HostString("tenantx.api.domain");
        var options = Microsoft.Extensions.Options.Options.Create(new TenantResolutionOptions
        {
            EnableHeader = false,
            EnableSubdomain = true,
            SubdomainPrefix = "tenant"
        });
        var middleware = new TenantResolutionMiddleware(_ => Task.CompletedTask, options);

        await middleware.Invoke(context);

        Assert.False(context.Items.ContainsKey(TenantContextKeys.TenantId));
    }

    [Fact]
    public async Task Invoke_DoesNotResolve_When_Localhost()
    {
        var context = new DefaultHttpContext();
        context.Request.Host = new HostString("localhost");
        var options = Microsoft.Extensions.Options.Options.Create(new TenantResolutionOptions
        {
            EnableHeader = false,
            EnableSubdomain = true,
            SubdomainPrefix = "tenant"
        });
        var middleware = new TenantResolutionMiddleware(_ => Task.CompletedTask, options);

        await middleware.Invoke(context);

        Assert.False(context.Items.ContainsKey(TenantContextKeys.TenantId));
    }
}
