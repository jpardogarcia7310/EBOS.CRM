using System.Net.Http.Json;
using EBOS.CRM.ApiTests.TestUtils;
using EBOS.CRM.Contracts.Requests.CRM.Quote;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.ConcurrencyTests.Fixtures;
using EBOS.CRM.ConcurrencyTests.Infrastructure;

namespace EBOS.CRM.ConcurrencyTests.Controllers.CRM.Quote;

public class QuoteConcurrencyTests(ConcurrencyWebApplicationFactory<Program> factory)
    : IClassFixture<ConcurrencyWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "Quote");

    [Fact]
    public async Task Quote_Concurrent_Reads_Work()
    {
        var baseUrl = $"/api/v{_version}/Quote";
        var id = await ConcurrencyEndpoints.GetFirstIdAsync(_client, _version, "Quote");

        await ConcurrencyHelper.AssertReadConcurrencyAsync(_client, baseUrl, id);
    }

    [Fact]
    public async Task Quote_Concurrent_Writes_Work()
    {
        var baseUrl = $"/api/v{_version}/Quote";
        var id = await ConcurrencyEndpoints.GetFirstIdAsync(_client, _version, "Quote");
        var payloads = await ConcurrencyPayloads.GetPayloadFactoriesAsync(_client, _version, "Quote");

        await ConcurrencyHelper.AssertWriteConcurrencyAsync(_client, baseUrl, id, payloads);
    }

    [Fact]
    public async Task Quote_ApproveReject_ReturnsSuccess()
    {
        var id = await ConcurrencyEndpoints.GetFirstIdAsync(_client, _version, "Quote");

        var approveResponse = await _client.PostAsJsonAsync($"/api/v{_version}/Quote/{id}/approve",
            new ApproveQuoteRequest(1, "Approve concurrency", null));
        approveResponse.EnsureSuccessStatusCode();

        var rejectResponse = await _client.PostAsJsonAsync($"/api/v{_version}/Quote/{id}/reject",
            new RejectQuoteRequest(1, "Reject concurrency", null));
        rejectResponse.EnsureSuccessStatusCode();

        var dto = await rejectResponse.Content.ReadFromJsonAsync<QuoteResponse>();
        Assert.NotNull(dto);
    }
}
