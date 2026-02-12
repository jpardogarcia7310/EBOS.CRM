using System.Net.Http.Json;
using EBOS.CRM.ApiTests.TestUtils;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.ConcurrencyTests.Fixtures;

namespace EBOS.CRM.ConcurrencyTests.Controllers.CRM.Forecast;

public class ForecastConcurrencyTests(ConcurrencyWebApplicationFactory<Program> factory)
    : IClassFixture<ConcurrencyWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "Forecast");

    [Fact]
    public async Task Forecast_Read_ReturnsSuccess()
    {
        var response = await _client.GetAsync($"/api/v{_version}/forecast?tenantId=1");
        response.EnsureSuccessStatusCode();

        var summary = await response.Content.ReadFromJsonAsync<ForecastSummaryResponse>();
        Assert.NotNull(summary);
    }
}
