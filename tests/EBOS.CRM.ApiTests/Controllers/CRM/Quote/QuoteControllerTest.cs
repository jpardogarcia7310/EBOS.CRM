using System.Net;
using System.Net.Http.Json;
using EBOS.CRM.Application.Contracts.Requests.CRM.Quote;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.ApiTests.TestUtils;
using EBOS.CRM.ApiTests.Fixtures;

namespace EBOS.CRM.ApiTests.Controllers.CRM.Quote;

public class QuoteControllerTest(CustomWebApplicationFactory<Program> factory) :
    IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "Quote");

    [Fact]
    public async Task GetAllQuotes_ReturnsSuccessAndList()
    {
        var response = await _client.GetAsync($"/api/v{_version}/Quote");
        response.EnsureSuccessStatusCode();

        var items = await response.Content.ReadItemsAsync<QuoteResponse>();
        Assert.NotNull(items);
        Assert.NotEmpty(items);
    }

    [Fact]
    public async Task GetQuoteById_ExistingId_ReturnsQuote()
    {
        var id = await ControllerTestHelper.GetFirstIdAsync<QuoteResponse>(
            _client, $"/api/v{_version}/Quote", x => x.Id);

        var response = await _client.GetAsync($"/api/v{_version}/Quote/{id}");
        response.EnsureSuccessStatusCode();

        var item = await response.Content.ReadFromJsonAsync<QuoteResponse>();
        Assert.NotNull(item);
        Assert.Equal(id, item.Id);
    }

    [Fact]
    public async Task GetQuoteById_NonExistingId_ReturnsNotFound()
    {
        var id = await ControllerTestHelper.GetFirstIdAsync<QuoteResponse>(
            _client, $"/api/v{_version}/Quote", x => x.Id);

        var response = await _client.GetAsync($"/api/v{_version}/Quote/{id + 9999}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task ApproveQuote_ReturnsUpdatedQuote()
    {
        var id = await ControllerTestHelper.GetFirstIdAsync<QuoteResponse>(
            _client, $"/api/v{_version}/Quote", x => x.Id);

        var request = new ApproveQuoteRequest(1, "Approved in tests", null);
        var response = await _client.PostAsJsonAsync($"/api/v{_version}/Quote/{id}/approve", request);
        response.EnsureSuccessStatusCode();

        var dto = await response.Content.ReadFromJsonAsync<QuoteResponse>();
        Assert.NotNull(dto);
        Assert.Equal("Approved", dto.Status);
    }

    [Fact]
    public async Task RejectQuote_ReturnsUpdatedQuote()
    {
        var id = await ControllerTestHelper.GetFirstIdAsync<QuoteResponse>(
            _client, $"/api/v{_version}/Quote", x => x.Id);

        var request = new RejectQuoteRequest(1, "Rejected in tests", null);
        var response = await _client.PostAsJsonAsync($"/api/v{_version}/Quote/{id}/reject", request);
        response.EnsureSuccessStatusCode();

        var dto = await response.Content.ReadFromJsonAsync<QuoteResponse>();
        Assert.NotNull(dto);
        Assert.Equal("Rejected", dto.Status);
    }
}
