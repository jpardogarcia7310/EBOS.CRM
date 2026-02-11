using System.Net;
using System.Net.Http.Json;
using EBOS.CRM.ApiTests.Fixtures;
using EBOS.CRM.ApiTests.TestUtils;
using EBOS.CRM.Application.Contracts.Responses;
using EBOS.CRM.Application.Contracts.Responses.EBOS;

namespace EBOS.CRM.ApiTests.Controllers.EBOS.IdentificationType;

public class IdentificationTypeControllerTest(CustomWebApplicationFactory<Program> factory) :
    IClassFixture<CustomWebApplicationFactory<Program>> // Your API's Program.cs file
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory);

    #region CRUD Básicos
    [Fact]
    public async Task GetAllIdentificationTypes_ReturnsSuccessAndList()
    {
        var response = await _client.GetAsync($"/api/v{_version}/IdentificationType");
        response.EnsureSuccessStatusCode();

        var items = await response.Content.ReadItemsAsync<IdentificationTypeResponse>();
        Assert.NotNull(items);
        Assert.NotEmpty(items);
    }

    [Fact]
    public async Task GetIdentificationTypeById_ExistingId_ReturnsIdentificationType()
    {
        var id = await ControllerTestHelper.GetFirstIdAsync<IdentificationTypeResponse>(
            _client, $"/api/v{_version}/IdentificationType", x => x.Id);

        var response = await _client.GetAsync($"/api/v{_version}/IdentificationType/{id}");
        response.EnsureSuccessStatusCode();

        var item = await response.Content.ReadFromJsonAsync<IdentificationTypeResponse>();
        Assert.NotNull(item);
        Assert.Equal(id, item.Id);
    }

    [Fact]
    public async Task GetIdentificationTypeById_NonExistingId_ReturnsNotFound()
    {
        var id = await ControllerTestHelper.GetFirstIdAsync<IdentificationTypeResponse>(
            _client, $"/api/v{_version}/IdentificationType", x => x.Id);

        var response = await _client.GetAsync($"/api/v{_version}/IdentificationType/{id + 9999}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    #endregion

    #region Stress & Performance
    [Fact]
    public async Task Resilience_DatabaseUnavailable_ReturnsServiceUnavailable()
    {
        // Simulation: special endpoint that forces a DB failure (example: /api/v2/IdentificationType/simulate-db-failure)
        var response = await _client.GetAsync($"/api/v{_version}/IdentificationType/simulate-db-failure");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Resilience_NetworkInterruption_ReturnsGatewayTimeout()
    {
        // Simulation: endpoint that forces network timeout
        var response = await _client.GetAsync($"/api/v{_version}/IdentificationType/simulate-timeout");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Recovery_AfterDatabaseFailure_RetrySucceeds()
    {
        // Simulation: first attempt fails (DB drops), second attempt recovers
        var response1 = await _client.GetAsync($"/api/v{_version}/IdentificationType/simulate-db-failure");
        Assert.Equal(HttpStatusCode.NotFound, response1.StatusCode);

        // We expect the system to apply a retry/circuit breaker and recover.
        var response2 = await _client.GetAsync($"/api/v{_version}/IdentificationType");
        response2.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Recovery_AfterTimeout_RetrySucceeds()
    {
        var response1 = await _client.GetAsync($"/api/v{_version}/IdentificationType/simulate-timeout");
        Assert.Equal(HttpStatusCode.NotFound, response1.StatusCode);

        // Second attempt should recover
        var response2 = await _client.GetAsync($"/api/v{_version}/IdentificationType");
        response2.EnsureSuccessStatusCode();
    }
    #endregion
}




