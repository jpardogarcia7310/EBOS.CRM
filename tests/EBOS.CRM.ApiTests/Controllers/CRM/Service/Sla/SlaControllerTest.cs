using System.Net;
using System.Net.Http.Json;
using EBOS.CRM.ApiTests.Fixtures;
using EBOS.CRM.ApiTests.TestUtils;
using EBOS.CRM.Contracts.Responses.CRM;

namespace EBOS.CRM.ApiTests.Controllers.CRM.Service.Sla;

public class SlaControllerTest(CustomWebApplicationFactory<Program> factory)
    : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "Sla");

    [Fact]
    public async Task GetAll_ReturnsSuccessAndList()
    {
        var response = await _client.GetAsync($"/api/v{_version}/Sla");
        AssertStatus(response.StatusCode, HttpStatusCode.OK, HttpStatusCode.Unauthorized);
        if (response.StatusCode != HttpStatusCode.OK) return;

        var items = await response.Content.ReadItemsAsync<SlaResponse>();
        Assert.NotNull(items);
        Assert.NotEmpty(items);
    }

    [Fact]
    public async Task GetById_ExistingId_ReturnsItem()
    {
        var listResponse = await _client.GetAsync($"/api/v{_version}/Sla");
        AssertStatus(listResponse.StatusCode, HttpStatusCode.OK, HttpStatusCode.Unauthorized);
        if (listResponse.StatusCode != HttpStatusCode.OK) return;
        var list = await listResponse.Content.ReadItemsAsync<SlaResponse>();
        var first = list.FirstOrDefault();
        Assert.NotNull(first);

        var response = await _client.GetAsync($"/api/v{_version}/Sla/{first!.Id}");
        AssertStatus(response.StatusCode, HttpStatusCode.OK, HttpStatusCode.Unauthorized);
        if (response.StatusCode != HttpStatusCode.OK) return;
        var item = await response.Content.ReadFromJsonAsync<SlaResponse>();
        Assert.NotNull(item);
        Assert.Equal(first.Id, item!.Id);
    }

    [Fact]
    public async Task GetById_NonExistingId_ReturnsNotFound()
    {
        var response = await _client.GetAsync($"/api/v{_version}/Sla/999999999");
        AssertStatus(response.StatusCode, HttpStatusCode.NotFound, HttpStatusCode.Unauthorized);
    }

    private static void AssertStatus(HttpStatusCode actual, params HttpStatusCode[] expected) =>
        Assert.True(expected.Contains(actual), $"Unexpected status code: {actual}");
}
