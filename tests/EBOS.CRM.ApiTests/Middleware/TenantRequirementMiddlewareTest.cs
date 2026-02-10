using System.Net;
using EBOS.CRM.Api.Middleware;
using EBOS.CRM.ApiTests.Fixtures;
using EBOS.CRM.ApiTests.TestUtils;
using EBOS.CRM.Application.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Moq;

namespace EBOS.CRM.ApiTests.Middleware;

public class TenantRequirementMiddlewareTest(CustomWebApplicationFactory<Program> factory)
    : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory);

    [Fact]
    public async Task Invoke_Allows_Request_When_TenantId_Present()
    {
        var middleware = new TenantRequirementMiddleware(_ => Task.CompletedTask);
        var context = new DefaultHttpContext
        {
            Request =
            {
                Path = $"/api/v{_version}/Country"
            }
        };

        var currentUserMock = new Mock<ICurrentUserContext>();
        currentUserMock.SetupGet(x => x.TenantId).Returns(5);

        await middleware.Invoke(context, currentUserMock.Object);

        Assert.NotEqual((int)HttpStatusCode.BadRequest, context.Response.StatusCode);
    }

    [Fact]
    public async Task Invoke_Skips_When_NotApiPath()
    {
        var called = false;
        var middleware = new TenantRequirementMiddleware(_ =>
        {
            called = true;
            return Task.CompletedTask;
        });

        var context = new DefaultHttpContext
        {
            Request =
            {
                Path = "/health"
            }
        };

        var currentUserMock = new Mock<ICurrentUserContext>();
        currentUserMock.SetupGet(x => x.TenantId).Returns(0);

        await middleware.Invoke(context, currentUserMock.Object);

        Assert.True(called);
    }

    [Fact]
    public async Task Invoke_Skips_When_SwaggerPath()
    {
        var called = false;
        var middleware = new TenantRequirementMiddleware(_ =>
        {
            called = true;
            return Task.CompletedTask;
        });

        var context = new DefaultHttpContext
        {
            Request =
            {
                Path = "/swagger/index.html"
            }
        };

        var currentUserMock = new Mock<ICurrentUserContext>();
        currentUserMock.SetupGet(x => x.TenantId).Returns(0);

        await middleware.Invoke(context, currentUserMock.Object);

        Assert.True(called);
    }

    [Fact]
    public async Task Invoke_Skips_When_OptionsRequest()
    {
        var called = false;
        var middleware = new TenantRequirementMiddleware(_ =>
        {
            called = true;
            return Task.CompletedTask;
        });

        var context = new DefaultHttpContext
        {
            Request =
            {
                Path = $"/api/v{_version}/Country",
                Method = HttpMethods.Options
            }
        };

        var currentUserMock = new Mock<ICurrentUserContext>();
        currentUserMock.SetupGet(x => x.TenantId).Returns(0);

        await middleware.Invoke(context, currentUserMock.Object);

        Assert.True(called);
    }
}
