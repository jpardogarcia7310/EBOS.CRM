using EBOS.CRM.ApiTests.Fixtures;
using EBOS.CRM.ApiTests.TestUtils;
using EBOS.CRM.StressTests.Infrastructure;

namespace EBOS.CRM.StressTests.Controllers.CRM.AccountContactRole;

public class AccountContactRoleStressTests(CustomWebApplicationFactory<Program> factory)
    : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "AccountContactRole");

    [Fact]
    public async Task AccountContactRole_ReadEndpoints_Stress_Works()
    {
        await StressHelper.AssertEndpointStressAsync(_client,
            $"/api/v{_version}/AccountContactRole/by-account-contact/1?tenantId=1");
    }
}
