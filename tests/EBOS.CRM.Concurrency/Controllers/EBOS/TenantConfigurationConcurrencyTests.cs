using EBOS.CRM.Concurrency.Fixtures;
using EBOS.CRM.ApiTests.TestUtils;
using EBOS.CRM.Concurrency.Infrastructure;

namespace EBOS.CRM.Concurrency.Controllers.EBOS;

public class TenantConfigurationConcurrencyTests(ConcurrencyWebApplicationFactory<Program> factory)
    : IClassFixture<ConcurrencyWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "TenantConfiguration");

    [Fact]
    public async Task TenantConfiguration_ReadConcurrency_Works()
    {
        var baseUrl = $"/api/v{_version}/TenantConfiguration";
        var id = await ConcurrencyEndpoints.GetFirstIdAsync(_client, _version, "TenantConfiguration");

        await ConcurrencyHelper.AssertReadConcurrencyAsync(_client, baseUrl, id);
    }

    [Fact]
    public async Task TenantConfiguration_WriteConcurrency_Returns_NoServerErrors()
    {
        var baseUrl = $"/api/v{_version}/TenantConfiguration";
        var id = await ConcurrencyEndpoints.GetFirstIdAsync(_client, _version, "TenantConfiguration");

        var payloads = await ConcurrencyPayloads.GetPayloadFactoriesAsync(_client, _version, "TenantConfiguration");

        await ConcurrencyHelper.AssertWriteConcurrencyAsync(_client, baseUrl, id, payloads);
    }
}

