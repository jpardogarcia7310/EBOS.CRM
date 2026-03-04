using System.Net;
using System.Net.Http.Json;
using EBOS.CRM.ApiTests.Fixtures;
using EBOS.CRM.ApiTests.TestUtils;
using EBOS.CRM.Contracts.Responses.CRM;

namespace EBOS.CRM.ApiTests.Controllers.CRM.AccountHierarchy;

public class AccountHierarchyControllerTest(CustomWebApplicationFactory<Program> factory)
    : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "AccountHierarchy");
    private readonly string _corporateVersion = ApiVersionHelper.GetLatestVersion(factory, "CorporateCustomer");

    [Fact]
    public async Task GetByAccount_ReturnsSuccess()
    {
        var corporatesResponse = await _client.GetAsync($"/api/v{_corporateVersion}/CorporateCustomer?tenantId=1");
        AssertStatus(corporatesResponse.StatusCode, HttpStatusCode.OK, HttpStatusCode.Unauthorized);
        if (corporatesResponse.StatusCode != HttpStatusCode.OK) return;
        var corporates = await corporatesResponse.Content.ReadItemsAsync<CorporateCustomerResponse>();
        var firstCorporate = corporates.FirstOrDefault();
        Assert.NotNull(firstCorporate);

        var response = await _client.GetAsync(
            $"/api/v{_version}/AccountHierarchy/by-account/{firstCorporate!.Id}?tenantId=1");
        AssertStatus(response.StatusCode, HttpStatusCode.OK, HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetById_ExistingId_ReturnsItem()
    {
        var corporatesResponse = await _client.GetAsync($"/api/v{_corporateVersion}/CorporateCustomer?tenantId=1");
        AssertStatus(corporatesResponse.StatusCode, HttpStatusCode.OK, HttpStatusCode.Unauthorized);
        if (corporatesResponse.StatusCode != HttpStatusCode.OK) return;
        var corporates = await corporatesResponse.Content.ReadItemsAsync<CorporateCustomerResponse>();
        var firstCorporate = corporates.FirstOrDefault();
        Assert.NotNull(firstCorporate);

        var listResponse = await _client.GetAsync(
            $"/api/v{_version}/AccountHierarchy/by-account/{firstCorporate!.Id}?tenantId=1");
        AssertStatus(listResponse.StatusCode, HttpStatusCode.OK, HttpStatusCode.Unauthorized);
        if (listResponse.StatusCode != HttpStatusCode.OK) return;
        var list = await listResponse.Content.ReadItemsAsync<AccountHierarchyResponse>();
        var first = list.FirstOrDefault();
        Assert.NotNull(first);

        var response = await _client.GetAsync($"/api/v{_version}/AccountHierarchy/{first!.Id}");
        AssertStatus(response.StatusCode, HttpStatusCode.OK, HttpStatusCode.Unauthorized);
        if (response.StatusCode != HttpStatusCode.OK) return;
        var item = await response.Content.ReadFromJsonAsync<AccountHierarchyResponse>();
        Assert.NotNull(item);
        Assert.Equal(first.Id, item!.Id);
    }

    [Fact]
    public async Task GetById_NonExistingId_ReturnsNotFound()
    {
        var response = await _client.GetAsync($"/api/v{_version}/AccountHierarchy/999999999");
        AssertStatus(response.StatusCode, HttpStatusCode.NotFound, HttpStatusCode.Unauthorized);
    }

    private static void AssertStatus(HttpStatusCode actual, params HttpStatusCode[] expected) =>
        Assert.True(expected.Contains(actual), $"Unexpected status code: {actual}");
}
