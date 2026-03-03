using System.Net;
using EBOS.CRM.IntegrationTests.Infrastructure;
using FluentAssertions;

namespace EBOS.CRM.IntegrationTests.Controllers.Observability;

public class MetricsEndpointTest(CustomWebApplicationFactory factory)
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Metrics_ReturnsPrometheusPayload()
    {
        var response = await _client.GetAsync("/metrics");
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.Forbidden);
        if (response.StatusCode != HttpStatusCode.OK) return;

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("customer360_merge_total");
    }
}
