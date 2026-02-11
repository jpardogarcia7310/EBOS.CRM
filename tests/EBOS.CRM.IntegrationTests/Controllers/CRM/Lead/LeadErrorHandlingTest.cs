using System.Net;
using System.Net.Http.Json;
using EBOS.CRM.Application.Contracts.Requests.CRM.Lead;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.IntegrationTests.Infrastructure;
using EBOS.CRM.IntegrationTests.TestUtils;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace EBOS.CRM.IntegrationTests.Controllers.CRM.Lead;

public class LeadErrorHandlingTest(LeadErrorHandlingTest.FailingLeadFactory factory)
    : IClassFixture<LeadErrorHandlingTest.FailingLeadFactory>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "Lead");

    [Fact]
    public async Task GetAll_Returns_500_WhenRepositoryFails()
    {
        var response = await _client.GetAsync($"/api/v{_version}/Lead");
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problem.Should().NotBeNull();
        problem!.Status.Should().Be(500);
    }

    [Fact]
    public async Task Add_Returns_500_WhenRepositoryFails()
    {
        var request = new AddLeadRequest(
            TenantId: 1,
            Source: "Web",
            Status: "New",
            OwnerUserId: 1,
            CompanyName: "Failing Lead",
            ContactName: "Jane Doe",
            Email: "lead@example.com",
            Phone: "1234567890",
            EstimatedValue: 100m,
            Notes: null);

        var response = await _client.PostAsJsonAsync($"/api/v{_version}/Lead", request);
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task Update_Returns_500_WhenRepositoryFails()
    {
        var request = new UpdateLeadRequest(
            Id: 1,
            TenantId: 1,
            Source: "Web",
            Status: "New",
            OwnerUserId: 1,
            CompanyName: "Failing Lead",
            ContactName: "Jane Doe",
            Email: "lead@example.com",
            Phone: "1234567890",
            EstimatedValue: 100m,
            Notes: null);

        var response = await _client.PutAsJsonAsync($"/api/v{_version}/Lead/1", request);
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    public sealed class FailingLeadFactory : CustomWebApplicationFactory
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<ILeadRepository>();
                services.AddScoped<ILeadRepository, FailingLeadRepository>();
            });
        }
    }
}
