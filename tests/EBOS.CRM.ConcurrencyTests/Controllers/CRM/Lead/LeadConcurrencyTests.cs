using EBOS.CRM.ApiTests.Fixtures;
using EBOS.CRM.ApiTests.TestUtils;
using EBOS.CRM.ConcurrencyTests.Infrastructure;

namespace EBOS.CRM.ConcurrencyTests.Controllers.CRM.Lead;

public class LeadConcurrencyTests(ConcurrencyWebApplicationFactory<Program> factory)
    : IClassFixture<ConcurrencyWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "Lead");

    [Fact]
    public async Task Lead_Concurrent_Reads_Work()
    {
        var baseUrl = $"/api/v{_version}/Lead";
        var id = await ConcurrencyEndpoints.GetFirstIdAsync(_client, _version, "Lead");

        await ConcurrencyHelper.AssertConcurrentReadsAsync(_client, baseUrl, id);
    }

    [Fact]
    public async Task Lead_Concurrent_Writes_Work()
    {
        var baseUrl = $"/api/v{_version}/Lead";
        var payloads = await ConcurrencyPayloads.GetPayloadFactoriesAsync(_client, _version, "Lead");

        await ConcurrencyHelper.AssertConcurrentWritesAsync(_client, baseUrl, payloads);
    }
}
