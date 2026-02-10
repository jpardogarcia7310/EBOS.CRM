using EBOS.CRM.ApiTests.TestUtils;
using EBOS.CRM.ConcurrencyTests.Fixtures;
using EBOS.CRM.ConcurrencyTests.Infrastructure;

namespace EBOS.CRM.ConcurrencyTests.Controllers.CRM.CorporateCustomer;

public class CorporateCustomerConcurrencyTests(ConcurrencyWebApplicationFactory<Program> factory)
    : IClassFixture<ConcurrencyWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "CorporateCustomer");

    [Fact]
    public async Task CorporateCustomer_ReadConcurrency_Works()
    {
        var baseUrl = $"/api/v{_version}/CorporateCustomer";
        var id = await ConcurrencyEndpoints.GetFirstIdAsync(_client, _version, "CorporateCustomer");

        await ConcurrencyHelper.AssertReadConcurrencyAsync(_client, baseUrl, id);
    }

    [Fact]
    public async Task CorporateCustomer_WriteConcurrency_Returns_NoServerErrors()
    {
        var baseUrl = $"/api/v{_version}/CorporateCustomer";
        var id = await ConcurrencyEndpoints.GetFirstIdAsync(_client, _version, "CorporateCustomer");

        var payloads = await ConcurrencyPayloads.GetPayloadFactoriesAsync(_client, _version,
            "CorporateCustomer");

        await ConcurrencyHelper.AssertWriteConcurrencyAsync(_client, baseUrl, id, payloads);
    }
}

