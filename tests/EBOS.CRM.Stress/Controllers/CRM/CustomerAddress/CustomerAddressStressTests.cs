using EBOS.CRM.ApiTests.Fixtures;
using EBOS.CRM.ApiTests.TestUtils;
using EBOS.CRM.Stress.Infrastructure;

namespace EBOS.CRM.Stress.Controllers.CRM;

public class CustomerAddressStressTests(CustomWebApplicationFactory<Program> factory)
    : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "CustomerAddress");

    [Fact]
    public async Task CustomerAddress_Stress_Reads_Work()
    {
        var baseUrl = $"/api/v{_version}/CustomerAddress";
        var id = await StressEndpoints.GetFirstIdAsync(_client, _version, "CustomerAddress");

        await StressHelper.AssertReadStressAsync(_client, baseUrl, id);
    }

    [Fact]
    public async Task CustomerAddress_Stress_Writes_Return_NoServerErrors()
    {
        var baseUrl = $"/api/v{_version}/CustomerAddress";
        var id = await StressEndpoints.GetFirstIdAsync(_client, _version, "CustomerAddress");

        var payloads = await StressPayloads.GetPayloadFactoriesAsync(_client, _version, "CustomerAddress");

        await StressHelper.AssertWriteStressAsync(_client, baseUrl, id, payloads);
    }

    [Fact]
    public async Task CustomerAddress_Stress_Negative_Returns_ClientErrors()
    {
        var baseUrl = $"/api/v{_version}/CustomerAddress";
        var id = await StressEndpoints.GetFirstIdAsync(_client, _version, "CustomerAddress");

        await StressHelper.AssertNegativeStressAsync(_client, baseUrl, id);
    }
}


