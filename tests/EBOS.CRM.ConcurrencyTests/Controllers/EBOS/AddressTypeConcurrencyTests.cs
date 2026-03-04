using EBOS.CRM.ApiTests.TestUtils;
using EBOS.CRM.ConcurrencyTests.Fixtures;
using EBOS.CRM.ConcurrencyTests.Infrastructure;

namespace EBOS.CRM.ConcurrencyTests.Controllers.EBOS;

public class AddressTypeConcurrencyTests(ConcurrencyWebApplicationFactory<Program> factory)
    : IClassFixture<ConcurrencyWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "AddressType");

    [Fact]
    public async Task AddressType_ReadConcurrency_Works()
    {
        var baseUrl = $"/api/v{_version}/AddressType";
        var id = await ConcurrencyEndpoints.GetFirstIdAsync(_client, _version, "AddressType");

        await ConcurrencyHelper.AssertReadConcurrencyAsync(_client, baseUrl, id);
    }

    [Fact]
    public async Task AddressType_WriteConcurrency_Returns_NoServerErrors()
    {
        var baseUrl = $"/api/v{_version}/AddressType";
        var id = await ConcurrencyEndpoints.GetFirstIdAsync(_client, _version, "AddressType");

        var payloads = await ConcurrencyPayloads.GetPayloadFactoriesAsync(_client, _version,
            "AddressType");

        await ConcurrencyHelper.AssertWriteConcurrencyAsync(_client, baseUrl, id, payloads);
    }
}

