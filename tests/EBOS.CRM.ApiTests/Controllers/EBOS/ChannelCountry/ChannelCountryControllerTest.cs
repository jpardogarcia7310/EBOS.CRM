using System.Net;
using System.Net.Http.Json;
using EBOS.CRM.ApiTests.Fixtures;
using EBOS.CRM.ApiTests.TestUtils;
using EBOS.CRM.Contracts.Responses.EBOS;

namespace EBOS.CRM.ApiTests.Controllers.EBOS.ChannelCountry;

public class ChannelCountryControllerTest(CustomWebApplicationFactory<Program> factory)
    : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "ChannelCountry");

    [Fact]
    public async Task GetAll_ReturnsSuccessAndList()
    {
        var response = await _client.GetAsync($"/api/v{_version}/ChannelCountry");
        response.EnsureSuccessStatusCode();

        var items = await response.Content.ReadItemsAsync<ChannelCountryResponse>();
        Assert.NotNull(items);
    }

    [Fact]
    public async Task GetById_ExistingId_ReturnsItem()
    {
        var listResponse = await _client.GetAsync($"/api/v{_version}/ChannelCountry");
        listResponse.EnsureSuccessStatusCode();
        var list = await listResponse.Content.ReadItemsAsync<ChannelCountryResponse>();
        var first = list.FirstOrDefault();
        if (first is null) return;

        var response = await _client.GetAsync($"/api/v{_version}/ChannelCountry/{first.Id}");
        response.EnsureSuccessStatusCode();
        var item = await response.Content.ReadFromJsonAsync<ChannelCountryResponse>();
        Assert.NotNull(item);
        Assert.Equal(first.Id, item!.Id);
    }

    [Fact]
    public async Task GetById_NonExistingId_ReturnsNotFound()
    {
        var response = await _client.GetAsync($"/api/v{_version}/ChannelCountry/999999999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
