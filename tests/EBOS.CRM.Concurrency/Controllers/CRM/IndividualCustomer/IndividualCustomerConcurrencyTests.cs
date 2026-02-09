using EBOS.CRM.Concurrency.Fixtures;
using EBOS.CRM.ApiTests.TestUtils;
using EBOS.CRM.Concurrency.Infrastructure;

namespace EBOS.CRM.Concurrency.Controllers.CRM;

public class IndividualCustomerConcurrencyTests(ConcurrencyWebApplicationFactory<Program> factory)
    : IClassFixture<ConcurrencyWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "IndividualCustomer");

    [Fact]
    public async Task IndividualCustomer_ReadConcurrency_Works()
    {
        var baseUrl = $"/api/v{_version}/IndividualCustomer";
        var id = await ConcurrencyEndpoints.GetFirstIdAsync(_client, _version, "IndividualCustomer");

        await ConcurrencyHelper.AssertReadConcurrencyAsync(_client, baseUrl, id);
    }

    [Fact]
    public async Task IndividualCustomer_WriteConcurrency_Returns_NoServerErrors()
    {
        var baseUrl = $"/api/v{_version}/IndividualCustomer";
        var id = await ConcurrencyEndpoints.GetFirstIdAsync(_client, _version, "IndividualCustomer");

        var payloads = await ConcurrencyPayloads.GetPayloadFactoriesAsync(_client, _version, "IndividualCustomer");

        await ConcurrencyHelper.AssertWriteConcurrencyAsync(_client, baseUrl, id, payloads);
    }
}

