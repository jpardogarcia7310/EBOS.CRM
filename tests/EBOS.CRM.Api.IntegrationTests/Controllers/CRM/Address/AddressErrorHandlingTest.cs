using System.Net;
using System.Net.Http.Json;
using EBOS.CRM.Api.IntegrationTests.Infrastructure;
using EBOS.CRM.Api.IntegrationTests.TestUtils;
using EBOS.CRM.Application.Contracts.Requests.CRM;
using EBOS.CRM.Application.Contracts.Requests.CRM.Address;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace EBOS.CRM.Api.IntegrationTests.Controllers.CRM.Address;

public class AddressErrorHandlingTest(AddressErrorHandlingTest.FailingAddressFactory factory)
    : IClassFixture<AddressErrorHandlingTest.FailingAddressFactory>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory);

    [Fact]
    public async Task GetAll_Returns_500_WhenRepositoryFails()
    {
        var response = await _client.GetAsync($"/api/{_version}/Address");
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problem.Should().NotBeNull();
        problem.Status.Should().Be(500);
    }

    [Fact]
    public async Task Add_Returns_500_WhenRepositoryFails()
    {
        var request = new AddAddressRequest(
            Street: "Main St",
            ExternalNumber: "123",
            InternalNumber: null,
            BetweenStreet1: null,
            BetweenStreet2: null,
            Neighbourhood: "Center",
            City: "Quito",
            StateOrProvince: "Pichincha",
            PostalCode: "EC17001",
            GoogleMapsUrl: null,
            Latitude: "0",
            Longitude: "0",
            CountryId: 1,
            AddressTypeId: 1
        );

        var response = await _client.PostAsJsonAsync($"/api/{_version}/Address", request);
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task Update_Returns_500_WhenRepositoryFails()
    {
        var request = new UpdateAddressRequest(
            Street: "Main St",
            ExternalNumber: "123",
            InternalNumber: null,
            BetweenStreet1: null,
            BetweenStreet2: null,
            Neighbourhood: "Center",
            City: "Quito",
            StateOrProvince: "Pichincha",
            PostalCode: "EC17001",
            GoogleMapsUrl: null,
            Latitude: "0",
            Longitude: "0",
            CountryId: 1,
            AddressTypeId: 1
        );

        var response = await _client.PutAsJsonAsync($"/api/{_version}/Address/1", request);
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task Delete_Returns_500_WhenRepositoryFails()
    {
        var response = await _client.DeleteAsync($"/api/{_version}/Address/1");
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    public sealed class FailingAddressFactory : CustomWebApplicationFactory
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(IAddressRepository));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                services.AddScoped<IAddressRepository, FailingAddressRepository>();
            });
        }
    }
}





