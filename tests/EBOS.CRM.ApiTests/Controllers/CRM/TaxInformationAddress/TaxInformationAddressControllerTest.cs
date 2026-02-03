using System.Net;
using System.Net.Http.Json;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.ApiTests.TestUtils;
using EBOS.CRM.ApiTests.Fixtures;

namespace EBOS.CRM.ApiTests.Controllers.CRM.TaxInformationAddress;

public class TaxInformationAddressControllerTest(CustomWebApplicationFactory<Program> factory) :
    IClassFixture<CustomWebApplicationFactory<Program>> // Your API's Program.cs file
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "TaxInformationAddress");

    #region CRUD Básicos
    [Fact]
    public async Task GetAllTaxInformationAddresss_ReturnsSuccessAndList()
    {
        var response = await _client.GetAsync($"/api/v{_version}/TaxInformationAddress");
        response.EnsureSuccessStatusCode();

        var items = await response.Content.ReadItemsAsync<TaxInformationAddressResponse>();
        Assert.NotNull(items);
        Assert.NotEmpty(items);
    }

    [Fact]
    public async Task GetTaxInformationAddressById_ExistingId_ReturnsTaxInformationAddress()
    {
        var id = await ControllerTestHelper.GetFirstIdAsync<TaxInformationAddressResponse>(
            _client, $"/api/v{_version}/TaxInformationAddress", x => x.Id);

        var response = await _client.GetAsync($"/api/v{_version}/TaxInformationAddress/{id}");
        response.EnsureSuccessStatusCode();

        var item = await response.Content.ReadFromJsonAsync<TaxInformationAddressResponse>();
        Assert.NotNull(item);
        Assert.Equal(id, item.Id);
    }

    [Fact]
    public async Task GetTaxInformationAddressById_NonExistingId_ReturnsNotFound()
    {
        var id = await ControllerTestHelper.GetFirstIdAsync<TaxInformationAddressResponse>(
            _client, $"/api/v{_version}/TaxInformationAddress", x => x.Id);

        var response = await _client.GetAsync($"/api/v{_version}/TaxInformationAddress/{id + 9999}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    #endregion

    #region Stress & Performance
    [Fact]
    public async Task Resilience_DatabaseUnavailable_ReturnsServiceUnavailable()
    {
        var response = await _client.GetAsync($"/api/v{_version}/TaxInformationAddress/simulate-db-failure");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Resilience_NetworkInterruption_ReturnsGatewayTimeout()
    {
        var response = await _client.GetAsync($"/api/v{_version}/TaxInformationAddress/simulate-timeout");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Recovery_AfterDatabaseFailure_RetrySucceeds()
    {
        var response1 = await _client.GetAsync($"/api/v{_version}/TaxInformationAddress/simulate-db-failure");
        Assert.Equal(HttpStatusCode.NotFound, response1.StatusCode);

        var response2 = await _client.GetAsync($"/api/v{_version}/TaxInformationAddress");
        response2.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Recovery_AfterTimeout_RetrySucceeds()
    {
        var response1 = await _client.GetAsync($"/api/v{_version}/TaxInformationAddress/simulate-timeout");
        Assert.Equal(HttpStatusCode.NotFound, response1.StatusCode);

        var response2 = await _client.GetAsync($"/api/v{_version}/TaxInformationAddress");
        response2.EnsureSuccessStatusCode();
    }
    #endregion
}





