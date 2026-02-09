using EBOS.CRM.ApiTests.TestUtils;
using EBOS.CRM.ConcurrencyTests.Fixtures;
using EBOS.CRM.ConcurrencyTests.Infrastructure;

namespace EBOS.CRM.ConcurrencyTests.Controllers.EBOS;

public class TenantQuotaConcurrencyTests(ConcurrencyWebApplicationFactory<Program> factory)
    : IClassFixture<ConcurrencyWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "TenantQuota");

    [Fact]
    public async Task TenantQuota_ReadConcurrency_Works()
    {
        var baseUrl = $"/api/v{_version}/TenantQuota";
        var id = await ConcurrencyEndpoints.GetFirstIdAsync(_client, _version, "TenantQuota");

        await ConcurrencyHelper.AssertReadConcurrencyAsync(_client, baseUrl, id);
    }

    [Fact]
    public async Task TenantQuota_WriteConcurrency_Returns_NoServerErrors()
    {
        var baseUrl = $"/api/v{_version}/TenantQuota";
        var id = await ConcurrencyEndpoints.GetFirstIdAsync(_client, _version, "TenantQuota");

        var payloads = await ConcurrencyPayloads.GetPayloadFactoriesAsync(_client, _version, 
            "TenantQuota");

        await ConcurrencyHelper.AssertWriteConcurrencyAsync(_client, baseUrl, id, payloads);
    }
}

