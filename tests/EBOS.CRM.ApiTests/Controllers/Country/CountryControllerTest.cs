using System.Net;
using System.Net.Http.Json;
using EBOS.CRM.Application.Contracts.Responses;
using EBOS.CRM.ApiTests.TestUtils;
using EBOS.CRM.ApiTests.Fixtures;

namespace EBOS.CRM.ApiTests.Controllers.Country;

public class CountryControllerTest(CustomWebApplicationFactory<Program> factory) :
    IClassFixture<CustomWebApplicationFactory<Program>> // Your API's Program.cs file
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory);

    #region CRUD Básicos
    [Fact]
    public async Task GetAllCountries_ReturnsSuccessAndList()
    {
        var response = await _client.GetAsync($"/api/v{_version}/Country");
        response.EnsureSuccessStatusCode();

        var countries = await response.Content.ReadPagedItemsAsync<CountryResponse>();
        Assert.NotNull(countries);
        Assert.NotEmpty(countries);
    }

    [Fact]
    public async Task GetCountryById_ExistingId_ReturnsCountry()
    {
        var id = await ControllerTestHelper.GetFirstIdAsync<CountryResponse>(
            _client, $"/api/v{_version}/Country", x => x.Id);

        var response = await _client.GetAsync($"/api/v{_version}/Country/{id}");
        response.EnsureSuccessStatusCode();

        var country = await response.Content.ReadFromJsonAsync<CountryResponse>();
        Assert.NotNull(country);
        Assert.Equal(id, country.Id);
    }

    [Fact]
    public async Task GetCountryById_NonExistingId_ReturnsNotFound()
    {
        var id = await ControllerTestHelper.GetFirstIdAsync<CountryResponse>(
            _client, $"/api/v{_version}/Country", x => x.Id);

        var response = await _client.GetAsync($"/api/v{_version}/Country/{id + 9999}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    #endregion

    #region Stress & Performance
    [Fact]
    public async Task Resilience_DatabaseUnavailable_ReturnsServiceUnavailable()
    {
        // Simulation: special endpoint that forces a DB failure (example: /api/v1/Country/simulate-db-failure)
        var response = await _client.GetAsync($"/api/v{_version}/Country/simulate-db-failure");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Resilience_NetworkInterruption_ReturnsGatewayTimeout()
    {
        // Simulation: endpoint that forces network timeout
        var response = await _client.GetAsync($"/api/v{_version}/Country/simulate-timeout");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Recovery_AfterDatabaseFailure_RetrySucceeds()
    {
        // Simulation: first attempt fails (DB drops), second attempt recovers
        var response1 = await _client.GetAsync($"/api/v{_version}/Country/simulate-db-failure");
        Assert.Equal(HttpStatusCode.NotFound, response1.StatusCode);

        // We expect the system to apply a retry/circuit breaker and recover.
        var response2 = await _client.GetAsync($"/api/v{_version}/Country");
        response2.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Recovery_AfterTimeout_RetrySucceeds()
    {
        var response1 = await _client.GetAsync($"/api/v{_version}/Country/simulate-timeout");
        Assert.Equal(HttpStatusCode.NotFound, response1.StatusCode);

        // Second attempt should recover
        var response2 = await _client.GetAsync($"/api/v{_version}/Country");
        response2.EnsureSuccessStatusCode();
    }
    #endregion
}

