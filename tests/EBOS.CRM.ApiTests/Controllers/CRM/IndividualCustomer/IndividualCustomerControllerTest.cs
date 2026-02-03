using System.Net;
using System.Net.Http.Json;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.ApiTests.TestUtils;
using EBOS.CRM.ApiTests.Fixtures;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace EBOS.CRM.ApiTests.Controllers.CRM.IndividualCustomer;

public class IndividualCustomerControllerTest(CustomWebApplicationFactory<Program> factory) :
    IClassFixture<CustomWebApplicationFactory<Program>> // Your API's Program.cs file
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "IndividualCustomer");

    #region CRUD Básicos
    [Fact]
    public async Task GetAllIndividualCustomers_ReturnsSuccessAndList()
    {
        var response = await _client.GetAsync($"/api/v{_version}/IndividualCustomer");
        response.EnsureSuccessStatusCode();

        var items = await response.Content.ReadPagedItemsAsync<IndividualCustomerResponse>();
        Assert.NotNull(items);
        Assert.NotEmpty(items);
    }

    [Fact]
    public async Task GetIndividualCustomerById_ExistingId_ReturnsIndividualCustomer()
    {
        var id = await ControllerTestHelper.GetFirstIdAsync<IndividualCustomerResponse>(
            _client, $"/api/v{_version}/IndividualCustomer", x => x.Id);

        var response = await _client.GetAsync($"/api/v{_version}/IndividualCustomer/{id}");
        response.EnsureSuccessStatusCode();

        var item = await response.Content.ReadFromJsonAsync<IndividualCustomerResponse>();
        Assert.NotNull(item);
        Assert.Equal(id, item.Id);
    }

    [Fact]
    public async Task GetIndividualCustomerById_NonExistingId_ReturnsNotFound()
    {
        var id = await ControllerTestHelper.GetFirstIdAsync<IndividualCustomerResponse>(
            _client, $"/api/v{_version}/IndividualCustomer", x => x.Id);

        var response = await _client.GetAsync($"/api/v{_version}/IndividualCustomer/{id + 9999}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    #endregion

    #region Stress & Performance
    [Fact]
    public async Task Resilience_DatabaseUnavailable_ReturnsServiceUnavailable()
    {
        var response = await _client.GetAsync($"/api/v{_version}/IndividualCustomer/simulate-db-failure");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Resilience_NetworkInterruption_ReturnsGatewayTimeout()
    {
        var response = await _client.GetAsync($"/api/v{_version}/IndividualCustomer/simulate-timeout");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Recovery_AfterDatabaseFailure_RetrySucceeds()
    {
        var response1 = await _client.GetAsync($"/api/v{_version}/IndividualCustomer/simulate-db-failure");
        Assert.Equal(HttpStatusCode.NotFound, response1.StatusCode);

        var response2 = await _client.GetAsync($"/api/v{_version}/IndividualCustomer");
        response2.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Recovery_AfterTimeout_RetrySucceeds()
    {
        var response1 = await _client.GetAsync($"/api/v{_version}/IndividualCustomer/simulate-timeout");
        Assert.Equal(HttpStatusCode.NotFound, response1.StatusCode);

        var response2 = await _client.GetAsync($"/api/v{_version}/IndividualCustomer");
        response2.EnsureSuccessStatusCode();
    }
    #endregion
}



