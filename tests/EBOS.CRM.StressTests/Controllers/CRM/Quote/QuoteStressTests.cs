using System.Net.Http.Json;
using EBOS.CRM.ApiTests.Fixtures;
using EBOS.CRM.ApiTests.TestUtils;
using EBOS.CRM.Application.Contracts.Requests.CRM.Quote;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.StressTests.Infrastructure;

namespace EBOS.CRM.StressTests.Controllers.CRM.Quote;

public class QuoteStressTests(CustomWebApplicationFactory<Program> factory)
    : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "Quote");

    [Fact]
    public async Task Quote_Stress_Reads_Work()
    {
        var baseUrl = $"/api/v{_version}/Quote";
        var id = await StressEndpoints.GetFirstIdAsync(_client, _version, "Quote");

        await StressHelper.AssertReadStressAsync(_client, baseUrl, id);
    }

    [Fact]
    public async Task Quote_Stress_Writes_Return_NoServerErrors()
    {
        var baseUrl = $"/api/v{_version}/Quote";
        var id = await StressEndpoints.GetFirstIdAsync(_client, _version, "Quote");

        var payloads = await StressPayloads.GetPayloadFactoriesAsync(_client, _version, "Quote");

        await StressHelper.AssertWriteStressAsync(_client, baseUrl, id, payloads);
    }

    [Fact]
    public async Task Quote_Stress_Negative_Returns_ClientErrors()
    {
        var baseUrl = $"/api/v{_version}/Quote";
        var id = await StressEndpoints.GetFirstIdAsync(_client, _version, "Quote");

        await StressHelper.AssertNegativeStressAsync(_client, baseUrl, id);
    }

    [Fact]
    public async Task Quote_ApproveReject_ReturnsSuccess()
    {
        var id = await StressEndpoints.GetFirstIdAsync(_client, _version, "Quote");

        var approveResponse = await _client.PostAsJsonAsync($"/api/v{_version}/Quote/{id}/approve",
            new ApproveQuoteRequest(1, "Approve stress", null));
        approveResponse.EnsureSuccessStatusCode();

        var rejectResponse = await _client.PostAsJsonAsync($"/api/v{_version}/Quote/{id}/reject",
            new RejectQuoteRequest(1, "Reject stress", null));
        rejectResponse.EnsureSuccessStatusCode();

        var dto = await rejectResponse.Content.ReadFromJsonAsync<QuoteResponse>();
        Assert.NotNull(dto);
    }
}
