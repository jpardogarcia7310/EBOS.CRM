using EBOS.CRM.ApiTests.Fixtures;

namespace EBOS.CRM.ApiTests.Controllers.Observability;

public class MetricsControllerTest(CustomWebApplicationFactory<Program> factory)
    : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task GetMetrics_ReturnsPrometheusPayload()
    {
        var response = await _client.GetAsync("/metrics");
        response.EnsureSuccessStatusCode();

        var contentType = response.Content.Headers.ContentType?.ToString();
        var payload = await response.Content.ReadAsStringAsync();

        Assert.NotNull(contentType);
        Assert.Contains("text/plain", contentType, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("customer360_merge_total", payload);
        Assert.Contains("customer360_dedupe_query_total", payload);
    }
}
