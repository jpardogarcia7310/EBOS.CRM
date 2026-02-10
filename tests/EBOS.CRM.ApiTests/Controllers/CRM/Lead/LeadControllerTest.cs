using System.Net;
using System.Net.Http.Json;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.ApiTests.TestUtils;
using EBOS.CRM.ApiTests.Fixtures;

namespace EBOS.CRM.ApiTests.Controllers.CRM.Lead;

public class LeadControllerTest(CustomWebApplicationFactory<Program> factory) :
    IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "Lead");

    [Fact]
    public async Task GetAllLeads_ReturnsSuccessAndList()
    {
        var response = await _client.GetAsync($"/api/v{_version}/Lead");
        response.EnsureSuccessStatusCode();

        var items = await response.Content.ReadItemsAsync<LeadResponse>();
        Assert.NotNull(items);
        Assert.NotEmpty(items);
    }

    [Fact]
    public async Task GetLeadById_ExistingId_ReturnsLead()
    {
        var id = await ControllerTestHelper.GetFirstIdAsync<LeadResponse>(
            _client, $"/api/v{_version}/Lead", x => x.Id);

        var response = await _client.GetAsync($"/api/v{_version}/Lead/{id}");
        response.EnsureSuccessStatusCode();

        var item = await response.Content.ReadFromJsonAsync<LeadResponse>();
        Assert.NotNull(item);
        Assert.Equal(id, item.Id);
    }

    [Fact]
    public async Task GetLeadById_NonExistingId_ReturnsNotFound()
    {
        var id = await ControllerTestHelper.GetFirstIdAsync<LeadResponse>(
            _client, $"/api/v{_version}/Lead", x => x.Id);

        var response = await _client.GetAsync($"/api/v{_version}/Lead/{id + 9999}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
