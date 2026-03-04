using EBOS.CRM.Api.Middleware;
using Microsoft.AspNetCore.Http;

namespace EBOS.CRM.ApiTests.Middleware;

public class CorrelationIdMiddlewareTest
{
    [Fact]
    public async Task InvokeAsync_UsesIncomingHeader_WhenPresent()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers[CorrelationIdMiddleware.HeaderName] = "cid-123";

        var middleware = new CorrelationIdMiddleware(_ => Task.CompletedTask);
        await middleware.InvokeAsync(context);

        Assert.Equal("cid-123", context.Items[CorrelationIdMiddleware.HeaderName]);
        Assert.Equal("cid-123", context.Response.Headers[CorrelationIdMiddleware.HeaderName]);
    }

    [Fact]
    public async Task InvokeAsync_GeneratesHeader_WhenMissing()
    {
        var context = new DefaultHttpContext();
        var middleware = new CorrelationIdMiddleware(_ => Task.CompletedTask);

        await middleware.InvokeAsync(context);

        var responseValue = context.Response.Headers[CorrelationIdMiddleware.HeaderName].ToString();
        Assert.False(string.IsNullOrWhiteSpace(responseValue));
        Assert.Equal(responseValue, context.Items[CorrelationIdMiddleware.HeaderName]);
    }
}
