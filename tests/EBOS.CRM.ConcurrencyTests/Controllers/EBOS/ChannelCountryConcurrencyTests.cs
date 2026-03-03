using EBOS.CRM.ApiTests.TestUtils;
using EBOS.CRM.ConcurrencyTests.Fixtures;
using EBOS.CRM.ConcurrencyTests.Infrastructure;

namespace EBOS.CRM.ConcurrencyTests.Controllers.EBOS;

public class ChannelCountryConcurrencyTests(ConcurrencyWebApplicationFactory<Program> factory)
    : IClassFixture<ConcurrencyWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "ChannelCountry");

    [Fact]
    public async Task ChannelCountry_ReadConcurrency_Works()
    {
        var baseUrl = $"/api/v{_version}/ChannelCountry";
        var id = await ConcurrencyEndpoints.GetFirstIdAsync(_client, _version, "ChannelCountry");

        await ConcurrencyHelper.AssertReadConcurrencyAsync(_client, baseUrl, id);
    }

    [Fact]
    public async Task ChannelCountry_WriteConcurrency_Returns_NoServerErrors()
    {
        var baseUrl = $"/api/v{_version}/ChannelCountry";
        var id = await ConcurrencyEndpoints.GetFirstIdAsync(_client, _version, "ChannelCountry");

        var payloads = await ConcurrencyPayloads.GetPayloadFactoriesAsync(_client, _version, "ChannelCountry");

        await ConcurrencyHelper.AssertWriteConcurrencyAsync(_client, baseUrl, id, payloads);
    }
}
