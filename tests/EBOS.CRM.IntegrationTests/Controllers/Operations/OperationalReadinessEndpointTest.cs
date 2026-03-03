using System.Net;
using EBOS.CRM.IntegrationTests.Infrastructure;
using FluentAssertions;

namespace EBOS.CRM.IntegrationTests.Controllers.Operations;

public class OperationalReadinessEndpointTest(CustomWebApplicationFactory factory)
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();
    private const string Version = "2.0";

    [Fact]
    public async Task Dashboard_ReturnsSuccess()
    {
        var response = await _client.GetAsync($"/api/v{Version}/OperationalReadiness/dashboard");
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.Forbidden);
        if (response.StatusCode != HttpStatusCode.OK) return;

        var content = await response.Content.ReadAsStringAsync();
        content.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Alerts_ReturnsSuccess()
    {
        var response = await _client.GetAsync($"/api/v{Version}/OperationalReadiness/alerts");
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.Forbidden);
        if (response.StatusCode != HttpStatusCode.OK) return;

        var content = await response.Content.ReadAsStringAsync();
        content.Should().NotBeNullOrWhiteSpace();
    }
}
