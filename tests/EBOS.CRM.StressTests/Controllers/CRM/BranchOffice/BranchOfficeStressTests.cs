using EBOS.CRM.ApiTests.Fixtures;
using EBOS.CRM.ApiTests.TestUtils;
using EBOS.CRM.StressTests.Infrastructure;

namespace EBOS.CRM.StressTests.Controllers.CRM.BranchOffice;

public class BranchOfficeStressTests(CustomWebApplicationFactory<Program> factory)
    : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "BranchOffice");

    [Fact]
    public async Task BranchOffice_Stress_Reads_Work()
    {
        var baseUrl = $"/api/v{_version}/BranchOffice";
        var id = await StressEndpoints.GetFirstIdAsync(_client, _version, "BranchOffice");

        await StressHelper.AssertReadStressAsync(_client, baseUrl, id);
    }

    [Fact]
    public async Task BranchOffice_Stress_Writes_Return_NoServerErrors()
    {
        var baseUrl = $"/api/v{_version}/BranchOffice";
        var id = await StressEndpoints.GetFirstIdAsync(_client, _version, "BranchOffice");

        var payloads = await StressPayloads.GetPayloadFactoriesAsync(_client, _version, "BranchOffice");

        await StressHelper.AssertWriteStressAsync(_client, baseUrl, id, payloads);
    }

    [Fact]
    public async Task BranchOffice_Stress_Negative_Returns_ClientErrors()
    {
        var baseUrl = $"/api/v{_version}/BranchOffice";
        var id = await StressEndpoints.GetFirstIdAsync(_client, _version, "BranchOffice");

        await StressHelper.AssertNegativeStressAsync(_client, baseUrl, id);
    }
}


