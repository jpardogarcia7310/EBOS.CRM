using System.Net;
using System.Net.Http.Json;
using EBOS.CRM.Application.Contracts.Requests.CRM.Quote;
using EBOS.CRM.IntegrationTests.Infrastructure;
using EBOS.CRM.IntegrationTests.TestUtils;
using FluentAssertions;

namespace EBOS.CRM.IntegrationTests.Controllers.CRM.Quote;

public class QuoteValidationTest(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "Quote");

    [Fact]
    public async Task GetById_Returns_400_WhenIdIsInvalid()
    {
        var response = await _client.GetAsync($"/api/v{_version}/Quote/-1");
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Add_Returns_400_WhenRequestIsInvalid()
    {
        var request = new AddQuoteRequest(
            TenantId: 1,
            OpportunityId: 0,
            Status: "",
            ReferenceNumber: new string('x', 60),
            SubtotalAmount: -1m,
            DiscountAmount: 100m,
            TotalAmount: -1m,
            ValidUntil: null,
            Notes: new string('x', 2501));

        var response = await _client.PostAsJsonAsync($"/api/v{_version}/Quote", request);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Update_Returns_400_WhenRequestIsInvalid()
    {
        var request = new UpdateQuoteRequest(
            Id: 0,
            TenantId: 1,
            OpportunityId: 0,
            Status: "",
            ReferenceNumber: new string('x', 60),
            SubtotalAmount: -1m,
            DiscountAmount: 100m,
            TotalAmount: -1m,
            ValidUntil: null,
            Notes: new string('x', 2501));

        var response = await _client.PutAsJsonAsync($"/api/v{_version}/Quote/1", request);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Delete_Returns_400_WhenIdIsInvalid()
    {
        var response = await _client.DeleteAsync($"/api/v{_version}/Quote/-1");
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
