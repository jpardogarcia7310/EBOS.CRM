using EBOS.CRM.Concurrency.Fixtures;
using EBOS.CRM.ApiTests.TestUtils;
using EBOS.CRM.Concurrency.Infrastructure;

namespace EBOS.CRM.Concurrency.Controllers.CRM;

public class TaxInformationConcurrencyTests(ConcurrencyWebApplicationFactory<Program> factory)
    : IClassFixture<ConcurrencyWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "TaxInformation");

    [Fact]
    public async Task TaxInformation_ReadConcurrency_Works()
    {
        var baseUrl = $"/api/v{_version}/TaxInformation";
        var id = await ConcurrencyEndpoints.GetFirstIdAsync(_client, _version, "TaxInformation");

        await ConcurrencyHelper.AssertReadConcurrencyAsync(_client, baseUrl, id);
    }

    [Fact]
    public async Task TaxInformation_WriteConcurrency_Returns_NoServerErrors()
    {
        var baseUrl = $"/api/v{_version}/TaxInformation";
        var id = await ConcurrencyEndpoints.GetFirstIdAsync(_client, _version, "TaxInformation");

        var payloads = await ConcurrencyPayloads.GetPayloadFactoriesAsync(_client, _version, "TaxInformation");

        await ConcurrencyHelper.AssertWriteConcurrencyAsync(_client, baseUrl, id, payloads);
    }
}

