using EBOS.CRM.Concurrency.Fixtures;
using EBOS.CRM.ApiTests.TestUtils;
using EBOS.CRM.Concurrency.Infrastructure;

namespace EBOS.CRM.Concurrency.Controllers.EBOS;

public class IdentificationTypeConcurrencyTests(ConcurrencyWebApplicationFactory<Program> factory)
    : IClassFixture<ConcurrencyWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "IdentificationType");

    [Fact]
    public async Task IdentificationType_ReadConcurrency_Works()
    {
        var baseUrl = $"/api/v{_version}/IdentificationType";
        var id = await ConcurrencyEndpoints.GetFirstIdAsync(_client, _version, "IdentificationType");

        await ConcurrencyHelper.AssertReadConcurrencyAsync(_client, baseUrl, id);
    }

    [Fact]
    public async Task IdentificationType_WriteConcurrency_Returns_NoServerErrors()
    {
        var baseUrl = $"/api/v{_version}/IdentificationType";
        var id = await ConcurrencyEndpoints.GetFirstIdAsync(_client, _version, "IdentificationType");

        var payloads = await ConcurrencyPayloads.GetPayloadFactoriesAsync(_client, _version, "IdentificationType");

        await ConcurrencyHelper.AssertWriteConcurrencyAsync(_client, baseUrl, id, payloads);
    }
}

