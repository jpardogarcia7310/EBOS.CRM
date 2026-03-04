using EBOS.CRM.ApiTests.TestUtils;
using EBOS.CRM.ConcurrencyTests.Fixtures;
using EBOS.CRM.ConcurrencyTests.Infrastructure;

namespace EBOS.CRM.ConcurrencyTests.Controllers.CRM.Service.Queue;

public class QueueConcurrencyTests(ConcurrencyWebApplicationFactory<Program> factory)
    : IClassFixture<ConcurrencyWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "Queue");

    [Fact]
    public async Task Queue_ReadConcurrency_Works()
    {
        var baseUrl = $"/api/v{_version}/Queue";
        var id = await ConcurrencyEndpoints.GetFirstIdAsync(_client, _version, "Queue");

        await ConcurrencyHelper.AssertReadConcurrencyAsync(_client, baseUrl, id);
    }

    [Fact]
    public async Task Queue_WriteConcurrency_Returns_NoServerErrors()
    {
        var baseUrl = $"/api/v{_version}/Queue";
        var id = await ConcurrencyEndpoints.GetFirstIdAsync(_client, _version, "Queue");

        var payloads = await ConcurrencyPayloads.GetPayloadFactoriesAsync(_client, _version, "Queue");

        await ConcurrencyHelper.AssertWriteConcurrencyAsync(_client, baseUrl, id, payloads);
    }
}
