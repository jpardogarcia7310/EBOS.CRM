using EBOS.CRM.ApiTests.Fixtures;
using EBOS.CRM.ApiTests.TestUtils;
using EBOS.CRM.StressTests.Infrastructure;

namespace EBOS.CRM.StressTests.Controllers.CRM.CustomerPreference;

public class CustomerPreferenceStressTests(CustomWebApplicationFactory<Program> factory)
    : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "CustomerPreference");

    [Fact]
    public async Task CustomerPreference_ReadEndpoints_Stress_Works()
    {
        await StressHelper.AssertEndpointStressAsync(_client,
            $"/api/v{_version}/CustomerPreference/by-customer/1?tenantId=1");
    }
}
