using System.Net;
using System.Net.Http.Json;
using EBOS.CRM.ApiTests.Fixtures;
using EBOS.CRM.ApiTests.TestUtils;
using EBOS.CRM.Application.Contracts.Responses.CRM;

namespace EBOS.CRM.ApiTests.Controllers.EBOS.TenantQuota;

public class TenantQuotaControllerTest(CustomWebApplicationFactory<Program> factory) :
    IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory);

    [Fact]
    public async Task GetAll_ReturnsSuccessAndList()
    {
        var response = await _client.GetAsync($"/api/v{_version}/TenantQuota");
        response.EnsureSuccessStatusCode();

        var items = await response.Content.ReadItemsAsync<TenantQuotaResponse>();
        Assert.NotNull(items);
        Assert.NotEmpty(items);
    }

    [Fact]
    public async Task GetById_ExistingId_ReturnsItem()
    {
        var id = await ControllerTestHelper.GetFirstIdAsync<TenantQuotaResponse>(
            _client, $"/api/v{_version}/TenantQuota", x => x.Id);

        var response = await _client.GetAsync($"/api/v{_version}/TenantQuota/{id}");
        response.EnsureSuccessStatusCode();

        var item = await response.Content.ReadFromJsonAsync<TenantQuotaResponse>();
        Assert.NotNull(item);
        Assert.Equal(id, item.Id);
    }

    [Fact]
    public async Task GetById_NonExistingId_ReturnsNotFound()
    {
        var id = await ControllerTestHelper.GetFirstIdAsync<TenantQuotaResponse>(
            _client, $"/api/v{_version}/TenantQuota", x => x.Id);

        var response = await _client.GetAsync($"/api/v{_version}/TenantQuota/{id + 9999}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
