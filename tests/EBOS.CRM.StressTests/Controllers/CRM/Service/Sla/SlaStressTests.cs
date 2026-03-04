using EBOS.CRM.ApiTests.Fixtures;
using EBOS.CRM.ApiTests.TestUtils;
using EBOS.CRM.StressTests.Infrastructure;

namespace EBOS.CRM.StressTests.Controllers.CRM.Service.Sla;

public class SlaStressTests(CustomWebApplicationFactory<Program> factory)
    : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "Sla");

    [Fact]
    public async Task Sla_Stress_Reads_Work()
    {
        var baseUrl = $"/api/v{_version}/Sla";
        var id = await StressEndpoints.GetFirstIdAsync(_client, _version, "Sla");
        await StressHelper.AssertReadStressAsync(_client, baseUrl, id);
    }

    [Fact]
    public async Task Sla_Stress_Writes_Return_NoServerErrors()
    {
        var baseUrl = $"/api/v{_version}/Sla";
        var id = await StressEndpoints.GetFirstIdAsync(_client, _version, "Sla");
        var payloads = await StressPayloads.GetPayloadFactoriesAsync(_client, _version, "Sla");
        await StressHelper.AssertWriteStressAsync(_client, baseUrl, id, payloads);
    }

    [Fact]
    public async Task Sla_Stress_Negative_Returns_ClientErrors()
    {
        var baseUrl = $"/api/v{_version}/Sla";
        var id = await StressEndpoints.GetFirstIdAsync(_client, _version, "Sla");
        await StressHelper.AssertNegativeStressAsync(_client, baseUrl, id);
    }
}
