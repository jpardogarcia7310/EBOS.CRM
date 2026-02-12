using System.Net.Http.Json;
using EBOS.CRM.ApiTests.TestUtils;
using EBOS.CRM.Contracts.Requests.CRM.Opportunity;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.ConcurrencyTests.Fixtures;
using EBOS.CRM.ConcurrencyTests.Infrastructure;

namespace EBOS.CRM.ConcurrencyTests.Controllers.CRM.Opportunity;

public class OpportunityConcurrencyTests(ConcurrencyWebApplicationFactory<Program> factory)
    : IClassFixture<ConcurrencyWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "Opportunity");

    [Fact]
    public async Task Opportunity_Concurrent_Reads_Work()
    {
        var baseUrl = $"/api/v{_version}/Opportunity";
        var id = await ConcurrencyEndpoints.GetFirstIdAsync(_client, _version, "Opportunity");

        await ConcurrencyHelper.AssertReadConcurrencyAsync(_client, baseUrl, id);
    }

    [Fact]
    public async Task Opportunity_Concurrent_Writes_Work()
    {
        var baseUrl = $"/api/v{_version}/Opportunity";
        var id = await ConcurrencyEndpoints.GetFirstIdAsync(_client, _version, "Opportunity");
        var payloads = await ConcurrencyPayloads.GetPayloadFactoriesAsync(_client, _version, "Opportunity");

        await ConcurrencyHelper.AssertWriteConcurrencyAsync(_client, baseUrl, id, payloads);
    }

    [Fact]
    public async Task Opportunity_WinLoss_ReturnsSuccess()
    {
        var id = await ConcurrencyEndpoints.GetFirstIdAsync(_client, _version, "Opportunity");
        var stageId = await ConcurrencyEndpoints.GetFirstIdAsync(_client, _version, "OpportunityStage");

        var winResponse = await _client.PostAsJsonAsync($"/api/v{_version}/Opportunity/{id}/win",
            new WinOpportunityRequest(1, stageId, "Win concurrency"));
        winResponse.EnsureSuccessStatusCode();

        var lossResponse = await _client.PostAsJsonAsync($"/api/v{_version}/Opportunity/{id}/loss",
            new LossOpportunityRequest(1, stageId, "Loss concurrency"));
        lossResponse.EnsureSuccessStatusCode();

        var dto = await lossResponse.Content.ReadFromJsonAsync<OpportunityResponse>();
        Assert.NotNull(dto);
    }
}
