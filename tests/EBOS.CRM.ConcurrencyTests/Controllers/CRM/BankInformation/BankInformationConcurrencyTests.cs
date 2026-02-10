using EBOS.CRM.ApiTests.TestUtils;
using EBOS.CRM.ConcurrencyTests.Fixtures;
using EBOS.CRM.ConcurrencyTests.Infrastructure;

namespace EBOS.CRM.ConcurrencyTests.Controllers.CRM.BankInformation;

public class BankInformationConcurrencyTests(ConcurrencyWebApplicationFactory<Program> factory)
    : IClassFixture<ConcurrencyWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "BankInformation");

    [Fact]
    public async Task BankInformation_ReadConcurrency_Works()
    {
        var baseUrl = $"/api/v{_version}/BankInformation";
        var id = await ConcurrencyEndpoints.GetFirstIdAsync(_client, _version, "BankInformation");

        await ConcurrencyHelper.AssertReadConcurrencyAsync(_client, baseUrl, id);
    }

    [Fact]
    public async Task BankInformation_WriteConcurrency_Returns_NoServerErrors()
    {
        var baseUrl = $"/api/v{_version}/BankInformation";
        var id = await ConcurrencyEndpoints.GetFirstIdAsync(_client, _version, "BankInformation");

        var payloads = await ConcurrencyPayloads.GetPayloadFactoriesAsync(_client, _version,
            "BankInformation");

        await ConcurrencyHelper.AssertWriteConcurrencyAsync(_client, baseUrl, id, payloads);
    }
}

