using System.Net;
using System.Net.Http.Json;
using EBOS.CRM.Api.IntegrationTests.Infrastructure;
using EBOS.CRM.Api.IntegrationTests.TestUtils;
using EBOS.CRM.Application.Contracts.Requests.CRM.CreditAccount;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace EBOS.CRM.Api.IntegrationTests.Controllers.CRM.CreditAccount;

public class CreditAccountErrorHandlingTest(CreditAccountErrorHandlingTest.FailingCreditAccountFactory factory)
    : IClassFixture<CreditAccountErrorHandlingTest.FailingCreditAccountFactory>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "CreditAccount");

    [Fact]
    public async Task GetAll_Returns_500_WhenRepositoryFails()
    {
        var response = await _client.GetAsync($"/api/v{_version}/CreditAccount");
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problem.Should().NotBeNull();
        problem!.Status.Should().Be(500);
    }

    [Fact]
    public async Task Add_Returns_500_WhenRepositoryFails()
    {
        var request = new AddCreditAccountRequest(
            TenantId: 1,
            MaxAmount: 1000m,
            UsedAmount: 0m,
            CustomerId: 1);

        var response = await _client.PostAsJsonAsync($"/api/v{_version}/CreditAccount", request);
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task Update_Returns_500_WhenRepositoryFails()
    {
        var request = new UpdateCreditAccountRequest(
            Id: 1,
            TenantId: 1,
            MaxAmount: 1000m,
            UsedAmount: 100m,
            CustomerId: 1);

        var response = await _client.PutAsJsonAsync($"/api/v{_version}/CreditAccount/1", request);
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task Delete_Returns_500_WhenRepositoryFails()
    {
        var response = await _client.DeleteAsync($"/api/v{_version}/CreditAccount/1");
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    public sealed class FailingCreditAccountFactory : CustomWebApplicationFactory
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(ICreditAccountRepository));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                services.AddScoped<ICreditAccountRepository, FailingCreditAccountRepository>();
            });
        }
    }
}



