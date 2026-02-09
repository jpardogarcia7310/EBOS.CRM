using System.Net;
using EBOS.CRM.Concurrency.Fixtures;
using EBOS.CRM.ApiTests.TestUtils;
using EBOS.CRM.Concurrency.Infrastructure;

namespace EBOS.CRM.Concurrency.Controllers;

public class AllEntitiesConcurrencyTests(ConcurrencyWebApplicationFactory<Program> factory)
    : IClassFixture<ConcurrencyWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory);

    [Fact]
    public async Task AllEntities_Concurrency_AllOperations()
    {
        foreach (var endpoint in ConcurrencyEndpoints.All)
        {
            var baseUrl = $"/api/v{_version}/{endpoint.Route}";
            var id = await ConcurrencyEndpoints.GetFirstIdAsync(_client, _version, endpoint.Route);
            var payloads = await ConcurrencyPayloads.GetPayloadFactoriesAsync(_client, _version, endpoint.Route);

            await ConcurrencyHelper.AssertReadConcurrencyAsync(_client, baseUrl, id);
            await ConcurrencyHelper.AssertWriteConcurrencyAsync(_client, baseUrl, id, payloads);
        }
    }
}
