using EBOS.CRM.ApiTests.Fixtures;
using EBOS.CRM.ApiTests.TestUtils;
using EBOS.CRM.StressTests.Infrastructure;

namespace EBOS.CRM.StressTests.Controllers.CRM.CustomerMerge;

public class CustomerMergeStressTests(CustomWebApplicationFactory<Program> factory)
    : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "CustomerMerge");

    [Fact]
    public async Task CustomerMerge_ReadEndpoints_Stress_Works()
    {
        await StressHelper.AssertEndpointStressAsync(_client,
            $"/api/v{_version}/CustomerMerge/duplicates?tenantId=1&pageNumber=1&pageSize=10",
            $"/api/v{_version}/CustomerMerge/history/by-winner/1?tenantId=1&pageNumber=1&pageSize=10",
            $"/api/v{_version}/CustomerMerge/history/by-merged/1?tenantId=1&pageNumber=1&pageSize=10");
    }
}
