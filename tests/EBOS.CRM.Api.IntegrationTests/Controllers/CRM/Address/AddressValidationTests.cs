using System.Net;
using System.Net.Http.Json;
using EBOS.CRM.Api.IntegrationTests.Infrastructure;
using EBOS.CRM.Application.Contracts.Requests.CRM;
using FluentAssertions;

namespace EBOS.CRM.Api.IntegrationTests.Controllers.CRM.Address;

public class AddressValidationTests(InMemoryAddressWebApplicationFactory factory)
    : IClassFixture<InMemoryAddressWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task GetById_Returns_400_WhenIdIsInvalid()
    {
        var response = await _client.GetAsync("/api/v2/Address/-1");
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Add_Returns_400_WhenRequestIsInvalid()
    {
        var request = new AddAddressRequest(
            IsPrimary: false,
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
            CustomerId: 0,
            CountryId: 0,
            AddressTypeId: 0
        );

        var response = await _client.PostAsJsonAsync("/api/v2/Address", request);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
