using System.Net;
using System.Net.Http.Json;
using EBOS.CRM.Application.Contracts.Requests.CRM.CreditTransaction;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.IntegrationTests.Infrastructure;
using EBOS.CRM.IntegrationTests.TestUtils;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace EBOS.CRM.IntegrationTests.Controllers.CRM.CreditTransaction;

public class CreditTransactionErrorHandlingTest(
    CreditTransactionErrorHandlingTest.FailingCreditTransactionFactory factory)
    : IClassFixture<CreditTransactionErrorHandlingTest.FailingCreditTransactionFactory>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "CreditTransaction");

    [Fact]
    public async Task GetAll_Returns_500_WhenRepositoryFails()
    {
        var response = await _client.GetAsync($"/api/v{_version}/CreditTransaction");
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problem.Should().NotBeNull();
        problem!.Status.Should().Be(500);
    }

    [Fact]
    public async Task Add_Returns_500_WhenRepositoryFails()
    {
        var request = new AddCreditTransactionRequest(
            TenantId: 1,
            Date: new DateTime(2024, 1, 1),
            Amount: 100m,
            Type: "Charge",
            ExternalReference: "REF-1",
            Comments: "Test",
            CreditAccountId: 1);

        var response = await _client.PostAsJsonAsync($"/api/v{_version}/CreditTransaction", request);
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task Update_Returns_500_WhenRepositoryFails()
    {
        var request = new UpdateCreditTransactionRequest(
            TenantId: 1,
            Date: new DateTime(2024, 1, 1),
            Amount: 100m,
            Type: "Charge",
            ExternalReference: "REF-1",
            Comments: "Test",
            CreditAccountId: 1);

        var response = await _client.PutAsJsonAsync($"/api/v{_version}/CreditTransaction/1", request);
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task Delete_Returns_500_WhenRepositoryFails()
    {
        var response = await _client.DeleteAsync($"/api/v{_version}/CreditTransaction/1");
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    public sealed class FailingCreditTransactionFactory : CustomWebApplicationFactory
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<ICreditTransactionRepository>();

                services.AddScoped<ICreditTransactionRepository, FailingCreditTransactionRepository>();
            });
        }
    }
}



