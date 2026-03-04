using System.Net;
using System.Net.Http.Json;
using EBOS.CRM.Contracts.Requests.CRM.BankInformation;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.IntegrationTests.Infrastructure;
using EBOS.CRM.IntegrationTests.TestUtils;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace EBOS.CRM.IntegrationTests.Controllers.CRM.BankInformation;

public class BankInformationErrorHandlingTest(BankInformationErrorHandlingTest.FailingBankInformationFactory factory)
    : IClassFixture<BankInformationErrorHandlingTest.FailingBankInformationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "BankInformation");

    private static AddBankInformationRequest BuildRequest() => new(
        TenantId: 1,
        Iban: "ES1200000000000000000000",
        Bic: "BANKESMM",
        BankName: "Bank",
        CustomerId: 1);

    [Fact]
    public async Task GetAll_Returns_500_WhenRepositoryFails()
    {
        var response = await _client.GetAsync($"/api/v{_version}/BankInformation");
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problem.Should().NotBeNull();
        problem!.Status.Should().Be(500);
    }

    [Fact]
    public async Task Add_Returns_500_WhenRepositoryFails()
    {
        var response = await _client.PostAsJsonAsync($"/api/v{_version}/BankInformation", BuildRequest());
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task Update_Returns_500_WhenRepositoryFails()
    {
        var response = await _client.PutAsJsonAsync($"/api/v{_version}/BankInformation/1", BuildRequest());
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task Delete_Returns_500_WhenRepositoryFails()
    {
        var response = await _client.DeleteAsync($"/api/v{_version}/BankInformation/1");
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    public sealed class FailingBankInformationFactory : CustomWebApplicationFactory
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<IBankInformationRepository>();

                services.AddScoped<IBankInformationRepository, FailingBankInformationRepository>();
            });
        }
    }
}



