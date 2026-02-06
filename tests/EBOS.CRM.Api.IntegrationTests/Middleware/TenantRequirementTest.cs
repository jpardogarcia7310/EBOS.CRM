using System.Net;
using EBOS.CRM.Api.IntegrationTests.Infrastructure;
using EBOS.CRM.Api.Constants;
using EBOS.CRM.Api.IntegrationTests.TestUtils;
using FluentAssertions;

namespace EBOS.CRM.Api.IntegrationTests.Middleware;

public class TenantRequirementTest(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory = factory;

    [Fact]
    public async Task MissingTenantId_Returns_400()
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Remove(HeaderNames.TenantId);
        var version = ApiVersionHelper.GetLatestVersion(_factory);

        var response = await client.GetAsync($"/api/v{version}/Country");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task InvalidTenantId_Returns_400()
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Remove(HeaderNames.TenantId);
        client.DefaultRequestHeaders.Add(HeaderNames.TenantId, "invalid");
        var version = ApiVersionHelper.GetLatestVersion(_factory);

        var response = await client.GetAsync($"/api/v{version}/Country");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
