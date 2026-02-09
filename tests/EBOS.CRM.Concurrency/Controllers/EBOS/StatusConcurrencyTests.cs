using EBOS.CRM.Concurrency.Fixtures;
using EBOS.CRM.ApiTests.TestUtils;
using EBOS.CRM.Concurrency.Infrastructure;

namespace EBOS.CRM.Concurrency.Controllers.EBOS;

public class StatusConcurrencyTests(ConcurrencyWebApplicationFactory<Program> factory)
    : IClassFixture<ConcurrencyWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "Status");

    [Fact]
    public async Task Status_ReadConcurrency_Works()
    {
        var baseUrl = $"/api/v{_version}/Status";
        var id = await ConcurrencyEndpoints.GetFirstIdAsync(_client, _version, "Status");

        await ConcurrencyHelper.AssertReadConcurrencyAsync(_client, baseUrl, id);
    }

    [Fact]
    public async Task Status_WriteConcurrency_Returns_NoServerErrors()
    {
        var baseUrl = $"/api/v{_version}/Status";
        var id = await ConcurrencyEndpoints.GetFirstIdAsync(_client, _version, "Status");

        var payloads = await ConcurrencyPayloads.GetPayloadFactoriesAsync(_client, _version, "Status");

        await ConcurrencyHelper.AssertWriteConcurrencyAsync(_client, baseUrl, id, payloads);
    }
}

