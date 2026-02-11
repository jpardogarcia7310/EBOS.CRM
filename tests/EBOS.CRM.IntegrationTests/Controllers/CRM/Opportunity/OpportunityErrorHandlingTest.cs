using System.Net;
using System.Net.Http.Json;
using EBOS.CRM.Application.Contracts.Requests.CRM.Opportunity;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.IntegrationTests.Infrastructure;
using EBOS.CRM.IntegrationTests.TestUtils;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace EBOS.CRM.IntegrationTests.Controllers.CRM.Opportunity;

public class OpportunityErrorHandlingTest(OpportunityErrorHandlingTest.FailingOpportunityFactory factory)
    : IClassFixture<OpportunityErrorHandlingTest.FailingOpportunityFactory>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "Opportunity");

    [Fact]
    public async Task GetAll_Returns_500_WhenRepositoryFails()
    {
        var response = await _client.GetAsync($"/api/v{_version}/Opportunity");
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problem.Should().NotBeNull();
        problem!.Status.Should().Be(500);
    }

    [Fact]
    public async Task Add_Returns_500_WhenRepositoryFails()
    {
        var request = new AddOpportunityRequest(
            TenantId: 1,
            Name: "Failing Opportunity",
            StageId: 1,
            OwnerUserId: 1,
            CustomerId: 1,
            ExpectedCloseDate: DateTime.UtcNow.AddDays(5),
            Amount: 100m,
            Probability: 0.2m,
            Source: "Web",
            SourceLeadId: null);

        var response = await _client.PostAsJsonAsync($"/api/v{_version}/Opportunity", request);
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task Update_Returns_500_WhenRepositoryFails()
    {
        var request = new UpdateOpportunityRequest(
            Id: 1,
            TenantId: 1,
            Name: "Failing Opportunity",
            StageId: 1,
            OwnerUserId: 1,
            CustomerId: 1,
            ExpectedCloseDate: DateTime.UtcNow.AddDays(5),
            Amount: 100m,
            Probability: 0.2m,
            Source: "Web",
            SourceLeadId: null,
            CloseReason: null);

        var response = await _client.PutAsJsonAsync($"/api/v{_version}/Opportunity/1", request);
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    public sealed class FailingOpportunityFactory : CustomWebApplicationFactory
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<IOpportunityRepository>();
                services.AddScoped<IOpportunityRepository, FailingOpportunityRepository>();
            });
        }
    }
}
