using EBOS.CRM.ApiTests.TestUtils;
using EBOS.CRM.ConcurrencyTests.Fixtures;
using EBOS.CRM.ConcurrencyTests.Infrastructure;

namespace EBOS.CRM.ConcurrencyTests.Controllers.CRM.CustomerMerge;

public class CustomerMergeConcurrencyTests(ConcurrencyWebApplicationFactory<Program> factory)
    : IClassFixture<ConcurrencyWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "CustomerMerge");

    [Fact]
    public async Task CustomerMerge_ReadEndpoints_Concurrency_Works()
    {
        await ConcurrencyHelper.AssertEndpointConcurrencyAsync(_client,
            $"/api/v{_version}/CustomerMerge/duplicates?tenantId=1&pageNumber=1&pageSize=10",
            $"/api/v{_version}/CustomerMerge/history/by-winner/1?tenantId=1&pageNumber=1&pageSize=10",
            $"/api/v{_version}/CustomerMerge/history/by-merged/1?tenantId=1&pageNumber=1&pageSize=10");
    }
}
