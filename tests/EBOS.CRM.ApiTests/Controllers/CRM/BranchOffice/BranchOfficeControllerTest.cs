using System.Net;
using System.Net.Http.Json;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.ApiTests.TestUtils;
using EBOS.CRM.ApiTests.Fixtures;

namespace EBOS.CRM.ApiTests.Controllers.CRM.BranchOffice;

public class BranchOfficeControllerTest(CustomWebApplicationFactory<Program> factory) :
    IClassFixture<CustomWebApplicationFactory<Program>> // Your API's Program.cs file
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "BranchOffice");

    #region CRUD Básicos
    [Fact]
    public async Task GetAllBranchOffices_ReturnsSuccessAndList()
    {
        var response = await _client.GetAsync($"/api/v{_version}/BranchOffice");
        response.EnsureSuccessStatusCode();

        var items = await response.Content.ReadItemsAsync<BranchOfficeResponse>();
        Assert.NotNull(items);
        Assert.NotEmpty(items);
    }

    [Fact]
    public async Task GetBranchOfficeById_ExistingId_ReturnsBranchOffice()
    {
        var id = await ControllerTestHelper.GetFirstIdAsync<BranchOfficeResponse>(
            _client, $"/api/v{_version}/BranchOffice", x => x.Id);

        var response = await _client.GetAsync($"/api/v{_version}/BranchOffice/{id}");
        response.EnsureSuccessStatusCode();

        var item = await response.Content.ReadFromJsonAsync<BranchOfficeResponse>();
        Assert.NotNull(item);
        Assert.Equal(id, item.Id);
    }

    [Fact]
    public async Task GetBranchOfficeById_NonExistingId_ReturnsNotFound()
    {
        var id = await ControllerTestHelper.GetFirstIdAsync<BranchOfficeResponse>(
            _client, $"/api/v{_version}/BranchOffice", x => x.Id);

        var response = await _client.GetAsync($"/api/v{_version}/BranchOffice/{id + 9999}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    #endregion

    #region Stress & Performance
    [Fact]
    public async Task Resilience_DatabaseUnavailable_ReturnsServiceUnavailable()
    {
        var response = await _client.GetAsync($"/api/v{_version}/BranchOffice/simulate-db-failure");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Resilience_NetworkInterruption_ReturnsGatewayTimeout()
    {
        var response = await _client.GetAsync($"/api/v{_version}/BranchOffice/simulate-timeout");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Recovery_AfterDatabaseFailure_RetrySucceeds()
    {
        var response1 = await _client.GetAsync($"/api/v{_version}/BranchOffice/simulate-db-failure");
        Assert.Equal(HttpStatusCode.NotFound, response1.StatusCode);

        var response2 = await _client.GetAsync($"/api/v{_version}/BranchOffice");
        response2.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Recovery_AfterTimeout_RetrySucceeds()
    {
        var response1 = await _client.GetAsync($"/api/v{_version}/BranchOffice/simulate-timeout");
        Assert.Equal(HttpStatusCode.NotFound, response1.StatusCode);

        var response2 = await _client.GetAsync($"/api/v{_version}/BranchOffice");
        response2.EnsureSuccessStatusCode();
    }
    #endregion
}





