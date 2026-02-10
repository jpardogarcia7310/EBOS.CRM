using EBOS.CRM.ApiTests.TestUtils;
using EBOS.CRM.ConcurrencyTests.Fixtures;
using EBOS.CRM.ConcurrencyTests.Infrastructure;

namespace EBOS.CRM.ConcurrencyTests.Controllers.CRM.CreditTransaction;

public class CreditTransactionConcurrencyTests(ConcurrencyWebApplicationFactory<Program> factory)
    : IClassFixture<ConcurrencyWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "CreditTransaction");

    [Fact]
    public async Task CreditTransaction_ReadConcurrency_Works()
    {
        var baseUrl = $"/api/v{_version}/CreditTransaction";
        var id = await ConcurrencyEndpoints.GetFirstIdAsync(_client, _version, "CreditTransaction");

        await ConcurrencyHelper.AssertReadConcurrencyAsync(_client, baseUrl, id);
    }

    [Fact]
    public async Task CreditTransaction_WriteConcurrency_Returns_NoServerErrors()
    {
        var baseUrl = $"/api/v{_version}/CreditTransaction";
        var id = await ConcurrencyEndpoints.GetFirstIdAsync(_client, _version, "CreditTransaction");

        var payloads = await ConcurrencyPayloads.GetPayloadFactoriesAsync(_client, _version,
            "CreditTransaction");

        await ConcurrencyHelper.AssertWriteConcurrencyAsync(_client, baseUrl, id, payloads);
    }
}

