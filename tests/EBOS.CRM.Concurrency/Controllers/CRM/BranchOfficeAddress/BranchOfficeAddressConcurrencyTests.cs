using EBOS.CRM.Concurrency.Fixtures;
using EBOS.CRM.ApiTests.TestUtils;
using EBOS.CRM.Concurrency.Infrastructure;

namespace EBOS.CRM.Concurrency.Controllers.CRM;

public class BranchOfficeAddressConcurrencyTests(ConcurrencyWebApplicationFactory<Program> factory)
    : IClassFixture<ConcurrencyWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "BranchOfficeAddress");

    [Fact]
    public async Task BranchOfficeAddress_ReadConcurrency_Works()
    {
        var baseUrl = $"/api/v{_version}/BranchOfficeAddress";
        var id = await ConcurrencyEndpoints.GetFirstIdAsync(_client, _version, "BranchOfficeAddress");

        await ConcurrencyHelper.AssertReadConcurrencyAsync(_client, baseUrl, id);
    }

    [Fact]
    public async Task BranchOfficeAddress_WriteConcurrency_Returns_NoServerErrors()
    {
        var baseUrl = $"/api/v{_version}/BranchOfficeAddress";
        var id = await ConcurrencyEndpoints.GetFirstIdAsync(_client, _version, "BranchOfficeAddress");

        var payloads = await ConcurrencyPayloads.GetPayloadFactoriesAsync(_client, _version, "BranchOfficeAddress");

        await ConcurrencyHelper.AssertWriteConcurrencyAsync(_client, baseUrl, id, payloads);
    }
}

