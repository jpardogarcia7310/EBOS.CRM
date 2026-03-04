using EBOS.CRM.ApiTests.TestUtils;
using EBOS.CRM.ConcurrencyTests.Fixtures;
using EBOS.CRM.ConcurrencyTests.Infrastructure;

namespace EBOS.CRM.ConcurrencyTests.Controllers.CRM.CreditAccount;

public class CreditAccountConcurrencyTests(ConcurrencyWebApplicationFactory<Program> factory)
    : IClassFixture<ConcurrencyWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "CreditAccount");

    [Fact]
    public async Task CreditAccount_ReadConcurrency_Works()
    {
        var baseUrl = $"/api/v{_version}/CreditAccount";
        var id = await ConcurrencyEndpoints.GetFirstIdAsync(_client, _version, "CreditAccount");

        await ConcurrencyHelper.AssertReadConcurrencyAsync(_client, baseUrl, id);
    }

    [Fact]
    public async Task CreditAccount_WriteConcurrency_Returns_NoServerErrors()
    {
        var baseUrl = $"/api/v{_version}/CreditAccount";
        var id = await ConcurrencyEndpoints.GetFirstIdAsync(_client, _version, "CreditAccount");

        var payloads = await ConcurrencyPayloads.GetPayloadFactoriesAsync(_client, _version,
            "CreditAccount");

        await ConcurrencyHelper.AssertWriteConcurrencyAsync(_client, baseUrl, id, payloads);
    }
}

