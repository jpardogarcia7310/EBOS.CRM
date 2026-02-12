using System.Net;
using System.Net.Http.Json;
using EBOS.CRM.ApiTests.Fixtures;
using EBOS.CRM.ApiTests.TestUtils;
using EBOS.CRM.Contracts.Responses.EBOS;

namespace EBOS.CRM.ApiTests.Controllers.EBOS.TenantUsageMetric;

public class TenantUsageMetricControllerTest(CustomWebApplicationFactory<Program> factory) :
    IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory);

    [Fact]
    public async Task GetAll_ReturnsSuccessAndList()
    {
        var response = await _client.GetAsync($"/api/v{_version}/TenantUsageMetric");
        response.EnsureSuccessStatusCode();

        var items = await response.Content.ReadItemsAsync<TenantUsageMetricResponse>();
        Assert.NotNull(items);
        Assert.NotEmpty(items);
    }

    [Fact]
    public async Task GetById_ExistingId_ReturnsItem()
    {
        var id = await ControllerTestHelper.GetFirstIdAsync<TenantUsageMetricResponse>(
            _client, $"/api/v{_version}/TenantUsageMetric", x => x.Id);

        var response = await _client.GetAsync($"/api/v{_version}/TenantUsageMetric/{id}");
        response.EnsureSuccessStatusCode();

        var item = await response.Content.ReadFromJsonAsync<TenantUsageMetricResponse>();
        Assert.NotNull(item);
        Assert.Equal(id, item.Id);
    }

    [Fact]
    public async Task GetById_NonExistingId_ReturnsNotFound()
    {
        var id = await ControllerTestHelper.GetFirstIdAsync<TenantUsageMetricResponse>(
            _client, $"/api/v{_version}/TenantUsageMetric", x => x.Id);

        var response = await _client.GetAsync($"/api/v{_version}/TenantUsageMetric/{id + 9999}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
