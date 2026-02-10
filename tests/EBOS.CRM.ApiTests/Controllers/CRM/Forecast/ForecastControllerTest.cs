using System.Net.Http.Json;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.ApiTests.Fixtures;
using EBOS.CRM.ApiTests.TestUtils;

namespace EBOS.CRM.ApiTests.Controllers.CRM.Forecast;

public class ForecastControllerTest(CustomWebApplicationFactory<Program> factory)
    : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "Forecast");

    [Fact]
    public async Task GetForecast_ReturnsSummary()
    {
        var response = await _client.GetAsync($"/api/v{_version}/forecast");
        response.EnsureSuccessStatusCode();

        var summary = await response.Content.ReadFromJsonAsync<ForecastSummaryResponse>();
        Assert.NotNull(summary);
        Assert.NotNull(summary.Stages);
    }
}
