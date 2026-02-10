using EBOS.CRM.ApiTests.TestUtils;
using EBOS.CRM.ConcurrencyTests.Fixtures;
using EBOS.CRM.ConcurrencyTests.Infrastructure;

namespace EBOS.CRM.ConcurrencyTests.Controllers.CRM.TaxInformationAddress;

public class TaxInformationAddressConcurrencyTests(ConcurrencyWebApplicationFactory<Program> factory)
    : IClassFixture<ConcurrencyWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "TaxInformationAddress");

    [Fact]
    public async Task TaxInformationAddress_ReadConcurrency_Works()
    {
        var baseUrl = $"/api/v{_version}/TaxInformationAddress";
        var id = await ConcurrencyEndpoints.GetFirstIdAsync(_client, _version, "TaxInformationAddress");

        await ConcurrencyHelper.AssertReadConcurrencyAsync(_client, baseUrl, id);
    }

    [Fact]
    public async Task TaxInformationAddress_WriteConcurrency_Returns_NoServerErrors()
    {
        var baseUrl = $"/api/v{_version}/TaxInformationAddress";
        var id = await ConcurrencyEndpoints.GetFirstIdAsync(_client, _version, "TaxInformationAddress");

        var payloads = await ConcurrencyPayloads.GetPayloadFactoriesAsync(_client, _version,
            "TaxInformationAddress");

        await ConcurrencyHelper.AssertWriteConcurrencyAsync(_client, baseUrl, id, payloads);
    }
}

