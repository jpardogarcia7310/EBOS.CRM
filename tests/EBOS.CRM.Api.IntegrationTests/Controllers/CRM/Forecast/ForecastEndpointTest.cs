using System.Net.Http.Json;
using EBOS.CRM.Api.IntegrationTests.Infrastructure;
using EBOS.CRM.Api.IntegrationTests.TestUtils;
using EBOS.CRM.Application.Contracts.Responses.CRM;

namespace EBOS.CRM.IntegrationTests.Controllers.CRM.Forecast;

public class ForecastEndpointTest(CustomWebApplicationFactory factory)
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "Forecast");

    [Fact]
    public async Task Forecast_ReturnsSummary()
    {
        var response = await _client.GetAsync($"/api/v{_version}/forecast");
        response.EnsureSuccessStatusCode();

        var summary = await response.Content.ReadFromJsonAsync<ForecastSummaryResponse>();
        Assert.NotNull(summary);
        Assert.NotNull(summary.Stages);
    }
}
