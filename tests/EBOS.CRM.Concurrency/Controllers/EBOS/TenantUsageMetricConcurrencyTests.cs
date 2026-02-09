using EBOS.CRM.Concurrency.Fixtures;
using EBOS.CRM.ApiTests.TestUtils;
using EBOS.CRM.Concurrency.Infrastructure;

namespace EBOS.CRM.Concurrency.Controllers.EBOS;

public class TenantUsageMetricConcurrencyTests(ConcurrencyWebApplicationFactory<Program> factory)
    : IClassFixture<ConcurrencyWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "TenantUsageMetric");

    [Fact]
    public async Task TenantUsageMetric_ReadConcurrency_Works()
    {
        var baseUrl = $"/api/v{_version}/TenantUsageMetric";
        var id = await ConcurrencyEndpoints.GetFirstIdAsync(_client, _version, "TenantUsageMetric");

        await ConcurrencyHelper.AssertReadConcurrencyAsync(_client, baseUrl, id);
    }

    [Fact]
    public async Task TenantUsageMetric_WriteConcurrency_Returns_NoServerErrors()
    {
        var baseUrl = $"/api/v{_version}/TenantUsageMetric";
        var id = await ConcurrencyEndpoints.GetFirstIdAsync(_client, _version, "TenantUsageMetric");

        var payloads = await ConcurrencyPayloads.GetPayloadFactoriesAsync(_client, _version, "TenantUsageMetric");

        await ConcurrencyHelper.AssertWriteConcurrencyAsync(_client, baseUrl, id, payloads);
    }
}

