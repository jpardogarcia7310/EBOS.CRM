using EBOS.CRM.ApiTests.TestUtils;
using EBOS.CRM.ConcurrencyTests.Fixtures;
using EBOS.CRM.ConcurrencyTests.Infrastructure;

namespace EBOS.CRM.ConcurrencyTests.Controllers.CRM.AccountContactRole;

public class AccountContactRoleConcurrencyTests(ConcurrencyWebApplicationFactory<Program> factory)
    : IClassFixture<ConcurrencyWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "AccountContactRole");

    [Fact]
    public async Task AccountContactRole_ReadEndpoints_Concurrency_Works()
    {
        await ConcurrencyHelper.AssertEndpointConcurrencyAsync(_client,
            $"/api/v{_version}/AccountContactRole/by-account-contact/1?tenantId=1");
    }
}
