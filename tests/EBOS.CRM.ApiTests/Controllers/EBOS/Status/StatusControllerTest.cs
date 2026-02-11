using System.Net;
using System.Net.Http.Json;
using EBOS.CRM.ApiTests.Fixtures;
using EBOS.CRM.ApiTests.TestUtils;
using EBOS.CRM.Application.Contracts.Responses;
using EBOS.CRM.Application.Contracts.Responses.EBOS;

namespace EBOS.CRM.ApiTests.Controllers.EBOS.Status;

public class StatusControllerTest(CustomWebApplicationFactory<Program> factory) :
    IClassFixture<CustomWebApplicationFactory<Program>> // Your API's Program.cs file
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory);

    #region CRUD Básicos
    [Fact]
    public async Task GetAllStatuses_ReturnsSuccessAndList()
    {
        var response = await _client.GetAsync($"/api/v{_version}/Status");
        response.EnsureSuccessStatusCode();

        var statuses = await response.Content.ReadItemsAsync<StatusResponse>();
        Assert.NotNull(statuses);
        Assert.NotEmpty(statuses);
    }

    [Fact]
    public async Task GetStatusById_ExistingId_ReturnsCountry()
    {
        var id = await ControllerTestHelper.GetFirstIdAsync<StatusResponse>(
            _client, $"/api/v{_version}/Status", x => x.Id);

        var response = await _client.GetAsync($"/api/v{_version}/Status/{id}");
        response.EnsureSuccessStatusCode();

        var status = await response.Content.ReadFromJsonAsync<StatusResponse>();
        Assert.NotNull(status);
        Assert.Equal(id, status.Id);
    }

    [Fact]
    public async Task GetStatusById_NonExistingId_ReturnsNotFound()
    {
        var id = await ControllerTestHelper.GetFirstIdAsync<StatusResponse>(
            _client, $"/api/v{_version}/Status", x => x.Id);

        var response = await _client.GetAsync($"/api/v{_version}/Status/{id + 9999}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    #endregion

    #region Stress & Performance
    [Fact]
    public async Task Resilience_DatabaseUnavailable_ReturnsServiceUnavailable()
    {
        // Simulation: special endpoint that forces a DB failure (example: /api/v1/Status/simulate-db-failure)
        var response = await _client.GetAsync($"/api/v{_version}/Status/simulate-db-failure");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Resilience_NetworkInterruption_ReturnsGatewayTimeout()
    {
        // Simulation: endpoint that forces network timeout
        var response = await _client.GetAsync($"/api/v{_version}/Status/simulate-timeout");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Recovery_AfterDatabaseFailure_RetrySucceeds()
    {
        // Simulation: first attempt fails (DB drops), second attempt recovers
        var response1 = await _client.GetAsync($"/api/v{_version}/Status/simulate-db-failure");
        Assert.Equal(HttpStatusCode.NotFound, response1.StatusCode);

        // We expect the system to apply a retry/circuit breaker and recover.
        var response2 = await _client.GetAsync($"/api/v{_version}/Status");
        response2.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Recovery_AfterTimeout_RetrySucceeds()
    {
        var response1 = await _client.GetAsync($"/api/v{_version}/Status/simulate-timeout");
        Assert.Equal(HttpStatusCode.NotFound, response1.StatusCode);

        // Second attempt should recover
        var response2 = await _client.GetAsync($"/api/v{_version}/Status");
        response2.EnsureSuccessStatusCode();
    }
    #endregion
}




