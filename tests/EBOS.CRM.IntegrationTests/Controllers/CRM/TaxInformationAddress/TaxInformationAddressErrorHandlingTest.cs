using System.Net;
using System.Net.Http.Json;
using EBOS.CRM.Contracts.Requests.CRM.TaxInformationAddress;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.IntegrationTests.Infrastructure;
using EBOS.CRM.IntegrationTests.TestUtils;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace EBOS.CRM.IntegrationTests.Controllers.CRM.TaxInformationAddress;

public class TaxInformationAddressErrorHandlingTest(
    TaxInformationAddressErrorHandlingTest.FailingTaxInformationAddressFactory factory)
    : IClassFixture<TaxInformationAddressErrorHandlingTest.FailingTaxInformationAddressFactory>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "TaxInformationAddress");

    [Fact]
    public async Task GetAll_Returns_500_WhenRepositoryFails()
    {
        var response = await _client.GetAsync($"/api/v{_version}/TaxInformationAddress");
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problem.Should().NotBeNull();
        problem.Status.Should().Be(500);
    }

    [Fact]
    public async Task Add_Returns_500_WhenRepositoryFails()
    {
        var request = new AddTaxInformationAddressRequest(
            TenantId: 1,
            TaxInformationId: 1,
            AddressId: 1,
            IsPrimary: true,
            ValidFrom: new DateTime(2024, 1, 1),
            ValidTo: null,
            IsCurrent: true);

        var response = await _client.PostAsJsonAsync($"/api/v{_version}/TaxInformationAddress", request);
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task Update_Returns_500_WhenRepositoryFails()
    {
        var request = new UpdateTaxInformationAddressRequest(
            TenantId: 1,
            TaxInformationId: 1,
            AddressId: 1,
            IsPrimary: true,
            ValidFrom: new DateTime(2024, 1, 1),
            ValidTo: null,
            IsCurrent: true);

        var response = await _client.PutAsJsonAsync($"/api/v{_version}/TaxInformationAddress/1", request);
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task Delete_Returns_500_WhenRepositoryFails()
    {
        var response = await _client.DeleteAsync($"/api/v{_version}/TaxInformationAddress/1");
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    public sealed class FailingTaxInformationAddressFactory : CustomWebApplicationFactory
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<ITaxInformationAddressRepository>();

                services.AddScoped<ITaxInformationAddressRepository, FailingTaxInformationAddressRepository>();
            });
        }
    }
}



