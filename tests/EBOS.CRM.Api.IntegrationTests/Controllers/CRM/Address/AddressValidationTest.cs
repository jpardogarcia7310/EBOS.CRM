using System.Net;
using System.Net.Http.Json;
using EBOS.CRM.Api.IntegrationTests.Infrastructure;
using EBOS.CRM.Api.IntegrationTests.TestUtils;
using EBOS.CRM.Application.Contracts.Requests.CRM;
using EBOS.CRM.Application.Contracts.Requests.CRM.Address;
using FluentAssertions;

namespace EBOS.CRM.Api.IntegrationTests.Controllers.CRM.Address;

public class AddressValidationTest(InMemoryAddressWebApplicationFactory factory)
    : IClassFixture<InMemoryAddressWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory);

    [Fact]
    public async Task GetById_Returns_400_WhenIdIsInvalid()
    {
        var response = await _client.GetAsync($"/api/{_version}/Address/-1");
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Add_Returns_400_WhenRequestIsInvalid()
    {
        var request = new AddAddressRequest(
            Street: "",
            ExternalNumber: "",
            InternalNumber: null,
            BetweenStreet1: null,
            BetweenStreet2: null,
            Neighbourhood: null,
            City: "",
            StateOrProvince: "",
            PostalCode: "",
            GoogleMapsUrl: "invalid-url",
            Latitude: "999",
            Longitude: "999",
            CountryId: 0,
            AddressTypeId: 0
        );

        var response = await _client.PostAsJsonAsync($"/api/{_version}/Address", request);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Update_Returns_400_WhenRequestIsInvalid()
    {
        var request = new UpdateAddressRequest(
            Street: "",
            ExternalNumber: "",
            InternalNumber: null,
            BetweenStreet1: null,
            BetweenStreet2: null,
            Neighbourhood: null,
            City: "",
            StateOrProvince: "",
            PostalCode: "",
            GoogleMapsUrl: "invalid-url",
            Latitude: "999",
            Longitude: "999",
            CountryId: 0,
            AddressTypeId: 0
        );

        var response = await _client.PutAsJsonAsync($"/api/{_version}/Address/1", request);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Delete_Returns_400_WhenIdIsInvalid()
    {
        var response = await _client.DeleteAsync($"/api/{_version}/Address/-1");
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}





