using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.ApiTests.TestUtils;
using EBOS.CRM.ApiTests.Fixtures;
using System.Net;
using System.Net.Http.Json;

namespace EBOS.CRM.ApiTests.Controllers.CRM.OpportunityStage;

public class OpportunityStageControllerTest(CustomWebApplicationFactory<Program> factory) :
    IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "OpportunityStage");

    [Fact]
    public async Task GetAllOpportunityStages_ReturnsSuccessAndList()
    {
        var response = await _client.GetAsync($"/api/v{_version}/OpportunityStage");
        response.EnsureSuccessStatusCode();

        var items = await response.Content.ReadItemsAsync<OpportunityStageResponse>();
        Assert.NotNull(items);
        Assert.NotEmpty(items);
    }

    [Fact]
    public async Task GetOpportunityStageById_ExistingId_ReturnsStage()
    {
        var id = await ControllerTestHelper.GetFirstIdAsync<OpportunityStageResponse>(
            _client, $"/api/v{_version}/OpportunityStage", x => x.Id);

        var response = await _client.GetAsync($"/api/v{_version}/OpportunityStage/{id}");
        response.EnsureSuccessStatusCode();

        var item = await response.Content.ReadFromJsonAsync<OpportunityStageResponse>();
        Assert.NotNull(item);
        Assert.Equal(id, item.Id);
    }

    [Fact]
    public async Task GetOpportunityStageById_NonExistingId_ReturnsNotFound()
    {
        var id = await ControllerTestHelper.GetFirstIdAsync<OpportunityStageResponse>(
            _client, $"/api/v{_version}/OpportunityStage", x => x.Id);

        var response = await _client.GetAsync($"/api/v{_version}/OpportunityStage/{id + 9999}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
