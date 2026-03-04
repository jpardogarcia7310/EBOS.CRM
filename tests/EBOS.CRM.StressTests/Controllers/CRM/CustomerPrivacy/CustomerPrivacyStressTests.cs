using EBOS.CRM.ApiTests.Fixtures;
using EBOS.CRM.ApiTests.TestUtils;
using EBOS.CRM.StressTests.Infrastructure;

namespace EBOS.CRM.StressTests.Controllers.CRM.CustomerPrivacy;

public class CustomerPrivacyStressTests(CustomWebApplicationFactory<Program> factory)
    : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "CustomerPrivacy");

    [Fact]
    public async Task CustomerPrivacy_ReadEndpoints_Stress_Works()
    {
        await StressHelper.AssertEndpointStressAsync(_client,
            $"/api/v{_version}/CustomerPrivacy/by-customer/1?tenantId=1",
            $"/api/v{_version}/CustomerPrivacy/by-status/PENDING?tenantId=1");
    }
}
