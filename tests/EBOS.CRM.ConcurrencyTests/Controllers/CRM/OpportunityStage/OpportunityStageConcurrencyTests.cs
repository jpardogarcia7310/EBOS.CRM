using EBOS.CRM.ApiTests.Fixtures;
using EBOS.CRM.ApiTests.TestUtils;
using EBOS.CRM.ConcurrencyTests.Infrastructure;

namespace EBOS.CRM.ConcurrencyTests.Controllers.CRM.OpportunityStage;

public class OpportunityStageConcurrencyTests(ConcurrencyWebApplicationFactory<Program> factory)
    : IClassFixture<ConcurrencyWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "OpportunityStage");

    [Fact]
    public async Task OpportunityStage_Concurrent_Reads_Work()
    {
        var baseUrl = $"/api/v{_version}/OpportunityStage";
        var id = await ConcurrencyEndpoints.GetFirstIdAsync(_client, _version, "OpportunityStage");

        await ConcurrencyHelper.AssertConcurrentReadsAsync(_client, baseUrl, id);
    }

    [Fact]
    public async Task OpportunityStage_Concurrent_Writes_Work()
    {
        var baseUrl = $"/api/v{_version}/OpportunityStage";
        var payloads = await ConcurrencyPayloads.GetPayloadFactoriesAsync(_client, _version, "OpportunityStage");

        await ConcurrencyHelper.AssertConcurrentWritesAsync(_client, baseUrl, payloads);
    }
}
