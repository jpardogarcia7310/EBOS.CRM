using EBOS.CRM.Concurrency.Fixtures;
using EBOS.CRM.ApiTests.TestUtils;
using EBOS.CRM.Concurrency.Infrastructure;

namespace EBOS.CRM.Concurrency.Controllers.CRM;

public class AddressConcurrencyTests(ConcurrencyWebApplicationFactory<Program> factory)
    : IClassFixture<ConcurrencyWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "Address");

    [Fact]
    public async Task Address_ReadConcurrency_Works()
    {
        var baseUrl = $"/api/v{_version}/Address";
        var id = await ConcurrencyEndpoints.GetFirstIdAsync(_client, _version, "Address");

        await ConcurrencyHelper.AssertReadConcurrencyAsync(_client, baseUrl, id);
    }

    [Fact]
    public async Task Address_WriteConcurrency_Returns_NoServerErrors()
    {
        var baseUrl = $"/api/v{_version}/Address";
        var id = await ConcurrencyEndpoints.GetFirstIdAsync(_client, _version, "Address");

        var payloads = await ConcurrencyPayloads.GetPayloadFactoriesAsync(_client, _version, "Address");

        await ConcurrencyHelper.AssertWriteConcurrencyAsync(_client, baseUrl, id, payloads);
    }
}

