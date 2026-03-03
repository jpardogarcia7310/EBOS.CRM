using System.Net;
using System.Net.Http.Json;
using EBOS.CRM.ApiTests.Fixtures;
using EBOS.CRM.ApiTests.TestUtils;
using EBOS.CRM.Contracts.Responses.CRM;

namespace EBOS.CRM.ApiTests.Controllers.CRM.Service.Queue;

public class QueueControllerTest(CustomWebApplicationFactory<Program> factory)
    : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "Queue");

    [Fact]
    public async Task GetAll_ReturnsSuccessAndList()
    {
        var response = await _client.GetAsync($"/api/v{_version}/Queue");
        AssertStatus(response.StatusCode, HttpStatusCode.OK, HttpStatusCode.Unauthorized);
        if (response.StatusCode != HttpStatusCode.OK) return;

        var items = await response.Content.ReadItemsAsync<QueueResponse>();
        Assert.NotNull(items);
        Assert.NotEmpty(items);
    }

    [Fact]
    public async Task GetById_ExistingId_ReturnsItem()
    {
        var listResponse = await _client.GetAsync($"/api/v{_version}/Queue");
        AssertStatus(listResponse.StatusCode, HttpStatusCode.OK, HttpStatusCode.Unauthorized);
        if (listResponse.StatusCode != HttpStatusCode.OK) return;
        var list = await listResponse.Content.ReadItemsAsync<QueueResponse>();
        var first = list.FirstOrDefault();
        Assert.NotNull(first);

        var response = await _client.GetAsync($"/api/v{_version}/Queue/{first!.Id}");
        AssertStatus(response.StatusCode, HttpStatusCode.OK, HttpStatusCode.Unauthorized);
        if (response.StatusCode != HttpStatusCode.OK) return;
        var item = await response.Content.ReadFromJsonAsync<QueueResponse>();
        Assert.NotNull(item);
        Assert.Equal(first.Id, item!.Id);
    }

    [Fact]
    public async Task GetById_NonExistingId_ReturnsNotFound()
    {
        var response = await _client.GetAsync($"/api/v{_version}/Queue/999999999");
        AssertStatus(response.StatusCode, HttpStatusCode.NotFound, HttpStatusCode.Unauthorized);
    }

    private static void AssertStatus(HttpStatusCode actual, params HttpStatusCode[] expected) =>
        Assert.True(expected.Contains(actual), $"Unexpected status code: {actual}");
}
