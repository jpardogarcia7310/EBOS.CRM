using EBOS.CRM.ApiTests.TestUtils;
using EBOS.CRM.ConcurrencyTests.Fixtures;
using EBOS.CRM.ConcurrencyTests.Infrastructure;

namespace EBOS.CRM.ConcurrencyTests.Controllers.CRM.AccountHierarchy;

public class AccountHierarchyConcurrencyTests(ConcurrencyWebApplicationFactory<Program> factory)
    : IClassFixture<ConcurrencyWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "AccountHierarchy");

    [Fact]
    public async Task AccountHierarchy_ReadEndpoints_Concurrency_Works()
    {
        await ConcurrencyHelper.AssertEndpointConcurrencyAsync(_client,
            $"/api/v{_version}/AccountHierarchy/by-account/1?tenantId=1");
    }
}
