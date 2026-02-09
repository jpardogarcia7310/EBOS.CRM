using EBOS.CRM.ApiTests.Fixtures;
using EBOS.CRM.ApiTests.TestUtils;
using EBOS.CRM.StressTests.Infrastructure;

namespace EBOS.CRM.StressTests.Controllers.CRM.CreditTransaction;

public class CreditTransactionStressTests(CustomWebApplicationFactory<Program> factory)
    : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "CreditTransaction");

    [Fact]
    public async Task CreditTransaction_Stress_Reads_Work()
    {
        var baseUrl = $"/api/v{_version}/CreditTransaction";
        var id = await StressEndpoints.GetFirstIdAsync(_client, _version, "CreditTransaction");

        await StressHelper.AssertReadStressAsync(_client, baseUrl, id);
    }

    [Fact]
    public async Task CreditTransaction_Stress_Writes_Return_NoServerErrors()
    {
        var baseUrl = $"/api/v{_version}/CreditTransaction";
        var id = await StressEndpoints.GetFirstIdAsync(_client, _version, "CreditTransaction");

        var payloads = await StressPayloads.GetPayloadFactoriesAsync(_client, _version, "CreditTransaction");

        await StressHelper.AssertWriteStressAsync(_client, baseUrl, id, payloads);
    }

    [Fact]
    public async Task CreditTransaction_Stress_Negative_Returns_ClientErrors()
    {
        var baseUrl = $"/api/v{_version}/CreditTransaction";
        var id = await StressEndpoints.GetFirstIdAsync(_client, _version, "CreditTransaction");

        await StressHelper.AssertNegativeStressAsync(_client, baseUrl, id);
    }
}


