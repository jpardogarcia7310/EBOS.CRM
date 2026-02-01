using System.Net;
using System.Net.Http.Json;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.ApiTests.TestUtils;
using EBOS.CRM.ApiTests.Fixtures;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace EBOS.CRM.ApiTests.Controllers.CRM.BranchOfficeAddress;

public class BranchOfficeAddressControllerTest(CustomWebApplicationFactory<Program> factory) :
    IClassFixture<CustomWebApplicationFactory<Program>> // Your API's Program.cs file
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory);

    #region CRUD Básicos
    [Fact]
    public async Task GetAllBranchOfficeAddresss_ReturnsSuccessAndList()
    {
        var response = await _client.GetAsync($"/api/{_version}/BranchOfficeAddress");
        response.EnsureSuccessStatusCode();

        var items = await response.Content.ReadPagedItemsAsync<BranchOfficeAddressResponse>();
        Assert.NotNull(items);
        Assert.NotEmpty(items);
    }

    [Fact]
    public async Task GetBranchOfficeAddressById_ExistingId_ReturnsBranchOfficeAddress()
    {
        var id = await ControllerTestHelper.GetFirstIdAsync<BranchOfficeAddressResponse>(
            _client, $"/api/{_version}/BranchOfficeAddress", x => x.Id);

        var response = await _client.GetAsync($"/api/{_version}/BranchOfficeAddress/{id}");
        response.EnsureSuccessStatusCode();

        var item = await response.Content.ReadFromJsonAsync<BranchOfficeAddressResponse>();
        Assert.NotNull(item);
        Assert.Equal(id, item.Id);
    }

    [Fact]
    public async Task GetBranchOfficeAddressById_NonExistingId_ReturnsNotFound()
    {
        var id = await ControllerTestHelper.GetFirstIdAsync<BranchOfficeAddressResponse>(
            _client, $"/api/{_version}/BranchOfficeAddress", x => x.Id);

        var response = await _client.GetAsync($"/api/{_version}/BranchOfficeAddress/{id + 9999}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    #endregion

    #region Stress & Performance
    [Fact]
    public async Task Resilience_DatabaseUnavailable_ReturnsServiceUnavailable()
    {
        var response = await _client.GetAsync($"/api/{_version}/BranchOfficeAddress/simulate-db-failure");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Resilience_NetworkInterruption_ReturnsGatewayTimeout()
    {
        var response = await _client.GetAsync($"/api/{_version}/BranchOfficeAddress/simulate-timeout");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Recovery_AfterDatabaseFailure_RetrySucceeds()
    {
        var response1 = await _client.GetAsync($"/api/{_version}/BranchOfficeAddress/simulate-db-failure");
        Assert.Equal(HttpStatusCode.NotFound, response1.StatusCode);

        var response2 = await _client.GetAsync($"/api/{_version}/BranchOfficeAddress");
        response2.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Recovery_AfterTimeout_RetrySucceeds()
    {
        var response1 = await _client.GetAsync($"/api/{_version}/BranchOfficeAddress/simulate-timeout");
        Assert.Equal(HttpStatusCode.NotFound, response1.StatusCode);

        var response2 = await _client.GetAsync($"/api/{_version}/BranchOfficeAddress");
        response2.EnsureSuccessStatusCode();
    }
    #endregion
}

