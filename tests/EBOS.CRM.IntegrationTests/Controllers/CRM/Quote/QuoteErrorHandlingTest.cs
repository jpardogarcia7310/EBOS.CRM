using System.Net;
using System.Net.Http.Json;
using EBOS.CRM.Application.Contracts.Requests.CRM.Quote;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.IntegrationTests.Infrastructure;
using EBOS.CRM.IntegrationTests.TestUtils;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace EBOS.CRM.IntegrationTests.Controllers.CRM.Quote;

public class QuoteErrorHandlingTest(QuoteErrorHandlingTest.FailingQuoteFactory factory)
    : IClassFixture<QuoteErrorHandlingTest.FailingQuoteFactory>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "Quote");

    [Fact]
    public async Task GetAll_Returns_500_WhenRepositoryFails()
    {
        var response = await _client.GetAsync($"/api/v{_version}/Quote");
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problem.Should().NotBeNull();
        problem!.Status.Should().Be(500);
    }

    [Fact]
    public async Task Add_Returns_500_WhenRepositoryFails()
    {
        var request = new AddQuoteRequest(
            TenantId: 1,
            OpportunityId: 1,
            Status: "Draft",
            ReferenceNumber: "Q-FAIL",
            SubtotalAmount: 100m,
            DiscountAmount: 0m,
            TotalAmount: 100m,
            ValidUntil: DateTime.UtcNow.AddDays(10),
            Notes: null);

        var response = await _client.PostAsJsonAsync($"/api/v{_version}/Quote", request);
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task Update_Returns_500_WhenRepositoryFails()
    {
        var request = new UpdateQuoteRequest(
            Id: 1,
            TenantId: 1,
            OpportunityId: 1,
            Status: "Draft",
            ReferenceNumber: "Q-FAIL",
            SubtotalAmount: 100m,
            DiscountAmount: 0m,
            TotalAmount: 100m,
            ValidUntil: DateTime.UtcNow.AddDays(10),
            Notes: null);

        var response = await _client.PutAsJsonAsync($"/api/v{_version}/Quote/1", request);
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task Delete_Returns_500_WhenRepositoryFails()
    {
        var response = await _client.DeleteAsync($"/api/v{_version}/Quote/1");
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    public sealed class FailingQuoteFactory : CustomWebApplicationFactory
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<IQuoteRepository>();
                services.AddScoped<IQuoteRepository, FailingQuoteRepository>();
            });
        }
    }
}
