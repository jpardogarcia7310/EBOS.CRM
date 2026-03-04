using System.Net.Http.Json;
using EBOS.CRM.ApiTests.Fixtures;

namespace EBOS.CRM.ApiTests.Controllers.Operations;

public class OperationalReadinessControllerTest(CustomWebApplicationFactory<Program> factory)
    : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();
    private const string Version = "2.0";

    [Fact]
    public async Task GetDashboard_ReturnsSuccess()
    {
        var response = await _client.GetAsync($"/api/v{Version}/OperationalReadiness/dashboard");
        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<object>();
        Assert.NotNull(payload);
    }

    [Fact]
    public async Task GetAlerts_ReturnsSuccess()
    {
        var response = await _client.GetAsync($"/api/v{Version}/OperationalReadiness/alerts");
        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<object>();
        Assert.NotNull(payload);
    }
}
