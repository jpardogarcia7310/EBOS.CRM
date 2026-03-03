using EBOS.CRM.ApiTests.Fixtures;
using EBOS.CRM.ApiTests.TestUtils;
using EBOS.CRM.StressTests.Infrastructure;

namespace EBOS.CRM.StressTests.Controllers.EBOS.ChannelCountry;

public class ChannelCountryStressTests(CustomWebApplicationFactory<Program> factory)
    : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "ChannelCountry");

    [Fact]
    public async Task ChannelCountry_Stress_Reads_Work()
    {
        var baseUrl = $"/api/v{_version}/ChannelCountry";
        var id = await StressEndpoints.GetFirstIdAsync(_client, _version, "ChannelCountry");
        await StressHelper.AssertReadStressAsync(_client, baseUrl, id);
    }

    [Fact]
    public async Task ChannelCountry_Stress_Writes_Return_NoServerErrors()
    {
        var baseUrl = $"/api/v{_version}/ChannelCountry";
        var id = await StressEndpoints.GetFirstIdAsync(_client, _version, "ChannelCountry");
        var payloads = await StressPayloads.GetPayloadFactoriesAsync(_client, _version, "ChannelCountry");
        await StressHelper.AssertWriteStressAsync(_client, baseUrl, id, payloads);
    }

    [Fact]
    public async Task ChannelCountry_Stress_Negative_Returns_ClientErrors()
    {
        var baseUrl = $"/api/v{_version}/ChannelCountry";
        var id = await StressEndpoints.GetFirstIdAsync(_client, _version, "ChannelCountry");
        await StressHelper.AssertNegativeStressAsync(_client, baseUrl, id);
    }
}
