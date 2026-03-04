using EBOS.CRM.ApiTests.Fixtures;
using EBOS.CRM.ApiTests.TestUtils;
using EBOS.CRM.StressTests.Infrastructure;

namespace EBOS.CRM.StressTests.Controllers.Operations;

public class OperationalReadinessStressTests(CustomWebApplicationFactory<Program> factory)
    : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "OperationalReadiness");

    [Fact]
    public async Task OperationalReadiness_ReadStress_Works()
    {
        await StressHelper.AssertEndpointStressAsync(_client,
            $"/api/v{_version}/OperationalReadiness/dashboard",
            $"/api/v{_version}/OperationalReadiness/alerts");
    }
}
