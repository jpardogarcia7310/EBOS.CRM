using System.Net;
using EBOS.CRM.Api.Constants;
using EBOS.CRM.Api.IntegrationTests.Infrastructure;
using EBOS.CRM.Api.IntegrationTests.TestUtils;
using FluentAssertions;

namespace EBOS.CRM.Api.IntegrationTests.Middleware;

public class TenantResolutionSubdomainTest(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory = factory;

    [Fact]
    public async Task SubdomainTenantId_Allows_Request()
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Remove(HeaderNames.TenantId);
        var version = ApiVersionHelper.GetLatestVersion(_factory);
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v{version}/Country");
        request.Headers.Host = "tenant7.api.domain";

        var response = await client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
