using EBOS.CRM.ApiTests.Fixtures;
using EBOS.CRM.ApiTests.TestUtils;
using EBOS.CRM.ConcurrencyTests.Infrastructure;

namespace EBOS.CRM.ConcurrencyTests.Controllers.CRM.Quote;

public class QuoteConcurrencyTests(ConcurrencyWebApplicationFactory<Program> factory)
    : IClassFixture<ConcurrencyWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "Quote");

    [Fact]
    public async Task Quote_Concurrent_Reads_Work()
    {
        var baseUrl = $"/api/v{_version}/Quote";
        var id = await ConcurrencyEndpoints.GetFirstIdAsync(_client, _version, "Quote");

        await ConcurrencyHelper.AssertConcurrentReadsAsync(_client, baseUrl, id);
    }

    [Fact]
    public async Task Quote_Concurrent_Writes_Work()
    {
        var baseUrl = $"/api/v{_version}/Quote";
        var payloads = await ConcurrencyPayloads.GetPayloadFactoriesAsync(_client, _version, "Quote");

        await ConcurrencyHelper.AssertConcurrentWritesAsync(_client, baseUrl, payloads);
    }
}
