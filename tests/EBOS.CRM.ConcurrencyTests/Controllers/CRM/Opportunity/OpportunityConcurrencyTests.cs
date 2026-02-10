using EBOS.CRM.ApiTests.Fixtures;
using EBOS.CRM.ApiTests.TestUtils;
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
}
