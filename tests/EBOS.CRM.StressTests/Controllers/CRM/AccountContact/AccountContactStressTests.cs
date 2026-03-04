using EBOS.CRM.ApiTests.Fixtures;
using EBOS.CRM.ApiTests.TestUtils;
using EBOS.CRM.StressTests.Infrastructure;

namespace EBOS.CRM.StressTests.Controllers.CRM.AccountContact;

public class AccountContactStressTests(CustomWebApplicationFactory<Program> factory)
    : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "AccountContact");

    [Fact]
    public async Task AccountContact_ReadEndpoints_Stress_Works()
    {
        await StressHelper.AssertEndpointStressAsync(_client,
            $"/api/v{_version}/AccountContact?tenantId=1",
            $"/api/v{_version}/AccountContact/by-account/1?tenantId=1");
    }
}
