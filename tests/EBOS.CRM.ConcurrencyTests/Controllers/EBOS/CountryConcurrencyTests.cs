using EBOS.CRM.ApiTests.TestUtils;
using EBOS.CRM.ConcurrencyTests.Fixtures;
using EBOS.CRM.ConcurrencyTests.Infrastructure;

namespace EBOS.CRM.ConcurrencyTests.Controllers.EBOS;

public class CountryConcurrencyTests(ConcurrencyWebApplicationFactory<Program> factory)
    : IClassFixture<ConcurrencyWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "Country");

    [Fact]
    public async Task Country_ReadConcurrency_Works()
    {
        var baseUrl = $"/api/v{_version}/Country";
        var id = await ConcurrencyEndpoints.GetFirstIdAsync(_client, _version, "Country");

        await ConcurrencyHelper.AssertReadConcurrencyAsync(_client, baseUrl, id);
    }

    [Fact]
    public async Task Country_WriteConcurrency_Returns_NoServerErrors()
    {
        var baseUrl = $"/api/v{_version}/Country";
        var id = await ConcurrencyEndpoints.GetFirstIdAsync(_client, _version, "Country");

        var payloads = await ConcurrencyPayloads.GetPayloadFactoriesAsync(_client, _version,
            "Country");

        await ConcurrencyHelper.AssertWriteConcurrencyAsync(_client, baseUrl, id, payloads);
    }
}

