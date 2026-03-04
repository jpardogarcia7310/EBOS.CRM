using EBOS.CRM.ApiTests.Fixtures;
using EBOS.CRM.StressTests.Infrastructure;

namespace EBOS.CRM.StressTests.Controllers.Observability;

public class MetricsStressTests(CustomWebApplicationFactory<Program> factory)
    : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Metrics_ReadStress_Works()
    {
        await StressHelper.AssertEndpointStressAsync(_client, "/metrics");
    }
}
