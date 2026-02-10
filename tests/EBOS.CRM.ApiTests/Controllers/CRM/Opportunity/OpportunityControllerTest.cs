using System.Net;
using System.Net.Http.Json;
using EBOS.CRM.Application.Contracts.Requests.CRM.Opportunity;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.ApiTests.TestUtils;
using EBOS.CRM.ApiTests.Fixtures;

namespace EBOS.CRM.ApiTests.Controllers.CRM.Opportunity;

public class OpportunityControllerTest(CustomWebApplicationFactory<Program> factory) :
    IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "Opportunity");

    [Fact]
    public async Task GetAllOpportunities_ReturnsSuccessAndList()
    {
        var response = await _client.GetAsync($"/api/v{_version}/Opportunity");
        response.EnsureSuccessStatusCode();

        var items = await response.Content.ReadItemsAsync<OpportunityResponse>();
        Assert.NotNull(items);
        Assert.NotEmpty(items);
    }

    [Fact]
    public async Task GetOpportunityById_ExistingId_ReturnsOpportunity()
    {
        var id = await ControllerTestHelper.GetFirstIdAsync<OpportunityResponse>(
            _client, $"/api/v{_version}/Opportunity", x => x.Id);

        var response = await _client.GetAsync($"/api/v{_version}/Opportunity/{id}");
        response.EnsureSuccessStatusCode();

        var item = await response.Content.ReadFromJsonAsync<OpportunityResponse>();
        Assert.NotNull(item);
        Assert.Equal(id, item.Id);
    }

    [Fact]
    public async Task GetOpportunityById_NonExistingId_ReturnsNotFound()
    {
        var id = await ControllerTestHelper.GetFirstIdAsync<OpportunityResponse>(
            _client, $"/api/v{_version}/Opportunity", x => x.Id);

        var response = await _client.GetAsync($"/api/v{_version}/Opportunity/{id + 9999}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task WinOpportunity_ReturnsUpdatedOpportunity()
    {
        var id = await ControllerTestHelper.GetFirstIdAsync<OpportunityResponse>(
            _client, $"/api/v{_version}/Opportunity", x => x.Id);
        var stageId = await ControllerTestHelper.GetFirstIdAsync<OpportunityStageResponse>(
            _client, $"/api/v{_version}/OpportunityStage", x => x.Id);

        var request = new WinOpportunityRequest(1, stageId, "Won in tests");
        var response = await _client.PostAsJsonAsync($"/api/v{_version}/Opportunity/{id}/win", request);
        response.EnsureSuccessStatusCode();

        var dto = await response.Content.ReadFromJsonAsync<OpportunityResponse>();
        Assert.NotNull(dto);
        Assert.Equal(id, dto.Id);
    }

    [Fact]
    public async Task LossOpportunity_ReturnsUpdatedOpportunity()
    {
        var id = await ControllerTestHelper.GetFirstIdAsync<OpportunityResponse>(
            _client, $"/api/v{_version}/Opportunity", x => x.Id);
        var stageId = await ControllerTestHelper.GetFirstIdAsync<OpportunityStageResponse>(
            _client, $"/api/v{_version}/OpportunityStage", x => x.Id);

        var request = new LossOpportunityRequest(1, stageId, "Lost in tests");
        var response = await _client.PostAsJsonAsync($"/api/v{_version}/Opportunity/{id}/loss", request);
        response.EnsureSuccessStatusCode();

        var dto = await response.Content.ReadFromJsonAsync<OpportunityResponse>();
        Assert.NotNull(dto);
        Assert.Equal(id, dto.Id);
    }
}
