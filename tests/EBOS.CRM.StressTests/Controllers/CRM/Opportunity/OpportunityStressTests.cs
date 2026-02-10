using EBOS.CRM.ApiTests.Fixtures;
using EBOS.CRM.ApiTests.TestUtils;
using EBOS.CRM.StressTests.Infrastructure;

namespace EBOS.CRM.StressTests.Controllers.CRM.Opportunity;

public class OpportunityStressTests(CustomWebApplicationFactory<Program> factory)
    : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "Opportunity");

    [Fact]
    public async Task Opportunity_Stress_Reads_Work()
    {
        var baseUrl = $"/api/v{_version}/Opportunity";
        var id = await StressEndpoints.GetFirstIdAsync(_client, _version, "Opportunity");

        await StressHelper.AssertReadStressAsync(_client, baseUrl, id);
    }

    [Fact]
    public async Task Opportunity_Stress_Writes_Return_NoServerErrors()
    {
        var baseUrl = $"/api/v{_version}/Opportunity";
        var id = await StressEndpoints.GetFirstIdAsync(_client, _version, "Opportunity");

        var payloads = await StressPayloads.GetPayloadFactoriesAsync(_client, _version, "Opportunity");

        await StressHelper.AssertWriteStressAsync(_client, baseUrl, id, payloads);
    }

    [Fact]
    public async Task Opportunity_Stress_Negative_Returns_ClientErrors()
    {
        var baseUrl = $"/api/v{_version}/Opportunity";
        var id = await StressEndpoints.GetFirstIdAsync(_client, _version, "Opportunity");

        await StressHelper.AssertNegativeStressAsync(_client, baseUrl, id);
    }
}
