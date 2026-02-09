using EBOS.CRM.ApiTests.Fixtures;
using EBOS.CRM.ApiTests.TestUtils;
using EBOS.CRM.Stress.Infrastructure;

namespace EBOS.CRM.Stress.Controllers.CRM;

public class CustomerStressTests(CustomWebApplicationFactory<Program> factory)
    : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "Customer");

    [Fact]
    public async Task Customer_Stress_Reads_Work()
    {
        var baseUrl = $"/api/v{_version}/Customer";
        var id = await StressEndpoints.GetFirstIdAsync(_client, _version, "Customer");

        await StressHelper.AssertReadStressAsync(_client, baseUrl, id);
    }

    [Fact]
    public async Task Customer_Stress_Writes_Return_NoServerErrors()
    {
        var baseUrl = $"/api/v{_version}/Customer";
        var id = await StressEndpoints.GetFirstIdAsync(_client, _version, "Customer");

        var payloads = await StressPayloads.GetPayloadFactoriesAsync(_client, _version, "Customer");

        await StressHelper.AssertWriteStressAsync(_client, baseUrl, id, payloads);
    }

    [Fact]
    public async Task Customer_Stress_Negative_Returns_ClientErrors()
    {
        var baseUrl = $"/api/v{_version}/Customer";
        var id = await StressEndpoints.GetFirstIdAsync(_client, _version, "Customer");

        await StressHelper.AssertNegativeStressAsync(_client, baseUrl, id);
    }
}


