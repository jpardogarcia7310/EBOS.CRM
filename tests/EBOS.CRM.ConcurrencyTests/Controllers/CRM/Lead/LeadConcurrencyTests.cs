using System.Net.Http.Json;
using EBOS.CRM.ApiTests.TestUtils;
using EBOS.CRM.Contracts.Requests.CRM.Lead;
using EBOS.CRM.ConcurrencyTests.Fixtures;
using EBOS.CRM.ConcurrencyTests.Infrastructure;

namespace EBOS.CRM.ConcurrencyTests.Controllers.CRM.Lead;

public class LeadConcurrencyTests(ConcurrencyWebApplicationFactory<Program> factory)
    : IClassFixture<ConcurrencyWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "Lead");

    [Fact]
    public async Task Lead_Concurrent_Reads_Work()
    {
        var baseUrl = $"/api/v{_version}/Lead";
        var id = await ConcurrencyEndpoints.GetFirstIdAsync(_client, _version, "Lead");

        await ConcurrencyHelper.AssertReadConcurrencyAsync(_client, baseUrl, id);
    }

    [Fact]
    public async Task Lead_Concurrent_Writes_Work()
    {
        var baseUrl = $"/api/v{_version}/Lead";
        var id = await ConcurrencyEndpoints.GetFirstIdAsync(_client, _version, "Lead");
        var payloads = await ConcurrencyPayloads.GetPayloadFactoriesAsync(_client, _version, "Lead");

        await ConcurrencyHelper.AssertWriteConcurrencyAsync(_client, baseUrl, id, payloads);
    }

    [Fact]
    public async Task Lead_DebtorCheck_ReturnsSuccess()
    {
        var request = new LeadDebtorCheckRequest(1, "lead@contoso.com", "1234567890", null, "Jane Doe");
        var response = await _client.PostAsJsonAsync($"/api/v{_version}/Lead/debtor-check", request);
        response.EnsureSuccessStatusCode();
    }
}
