using EBOS.CRM.ApiTests.Fixtures;
using EBOS.CRM.ApiTests.TestUtils;
using EBOS.CRM.StressTests.Infrastructure;

namespace EBOS.CRM.StressTests.Controllers.CRM.CorporateCustomer;

public class CorporateCustomerStressTests(CustomWebApplicationFactory<Program> factory)
    : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "CorporateCustomer");

    [Fact]
    public async Task CorporateCustomer_Stress_Reads_Work()
    {
        var baseUrl = $"/api/v{_version}/CorporateCustomer";
        var id = await StressEndpoints.GetFirstIdAsync(_client, _version, "CorporateCustomer");

        await StressHelper.AssertReadStressAsync(_client, baseUrl, id);
    }

    [Fact]
    public async Task CorporateCustomer_Stress_Writes_Return_NoServerErrors()
    {
        var baseUrl = $"/api/v{_version}/CorporateCustomer";
        var id = await StressEndpoints.GetFirstIdAsync(_client, _version, "CorporateCustomer");

        var payloads = await StressPayloads.GetPayloadFactoriesAsync(_client, _version, "CorporateCustomer");

        await StressHelper.AssertWriteStressAsync(_client, baseUrl, id, payloads);
    }

    [Fact]
    public async Task CorporateCustomer_Stress_Negative_Returns_ClientErrors()
    {
        var baseUrl = $"/api/v{_version}/CorporateCustomer";
        var id = await StressEndpoints.GetFirstIdAsync(_client, _version, "CorporateCustomer");

        await StressHelper.AssertNegativeStressAsync(_client, baseUrl, id);
    }
}


