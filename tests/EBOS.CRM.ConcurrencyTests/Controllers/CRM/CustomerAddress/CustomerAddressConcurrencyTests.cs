using EBOS.CRM.ApiTests.TestUtils;
using EBOS.CRM.ConcurrencyTests.Fixtures;
using EBOS.CRM.ConcurrencyTests.Infrastructure;

namespace EBOS.CRM.ConcurrencyTests.Controllers.CRM.CustomerAddress;

public class CustomerAddressConcurrencyTests(ConcurrencyWebApplicationFactory<Program> factory)
    : IClassFixture<ConcurrencyWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "CustomerAddress");

    [Fact]
    public async Task CustomerAddress_ReadConcurrency_Works()
    {
        var baseUrl = $"/api/v{_version}/CustomerAddress";
        var id = await ConcurrencyEndpoints.GetFirstIdAsync(_client, _version, "CustomerAddress");

        await ConcurrencyHelper.AssertReadConcurrencyAsync(_client, baseUrl, id);
    }

    [Fact]
    public async Task CustomerAddress_WriteConcurrency_Returns_NoServerErrors()
    {
        var baseUrl = $"/api/v{_version}/CustomerAddress";
        var id = await ConcurrencyEndpoints.GetFirstIdAsync(_client, _version, "CustomerAddress");

        var payloads = await ConcurrencyPayloads.GetPayloadFactoriesAsync(_client, _version, 
            "CustomerAddress");

        await ConcurrencyHelper.AssertWriteConcurrencyAsync(_client, baseUrl, id, payloads);
    }
}

