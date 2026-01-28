using System.Net;
using System.Net.Http.Json;
using EBOS.CRM.Api.IntegrationTests.Infrastructure;
using EBOS.CRM.Application.Contracts.Requests.CRM;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using FluentAssertions;

namespace EBOS.CRM.Api.IntegrationTests.Controllers.CRM.Address;

public class AddressTests : IClassFixture<InMemoryAddressWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AddressTests(InMemoryAddressWebApplicationFactory factory)
    {
        _client = factory.CreateClient();

        if (!factory.Repository.Items.Any())
        {
            factory.Repository.AddAsync(new Domain.Entities.CRM.Address
            {
                Id = 1,
                IsPrimary = false,
                Street = "Main St",
                ExternalNumber = "123",
                InternalNumber = null,
                BetweenStreet1 = null,
                BetweenStreet2 = null,
                Neighbourhood = "Center",
                City = "Quito",
                StateOrProvince = "Pichincha",
                PostalCode = "EC17001",
                GoogleMapsUrl = null,
                Latitude = 0,
                Longitude = 0,
                CustomerId = 1,
                CountryId = 1,
                AddressTypeId = 1
            }).GetAwaiter().GetResult();
        }
    }

    [Fact]
    public async Task GetAll_Returns_ListOfAddresses()
    {
        var response = await _client.GetAsync("/api/v2/Address");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var items = await response.Content.ReadFromJsonAsync<List<AddressResponse>>();
        items.Should().NotBeNull();
        items.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetById_Returns_Address_WhenExists()
    {
        var response = await _client.GetAsync("/api/v2/Address/1");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var item = await response.Content.ReadFromJsonAsync<AddressResponse>();
        item.Should().NotBeNull();
        item.Street.Should().Be("Main St");
    }

    [Fact]
    public async Task Add_Returns_Address_WhenValid()
    {
        var request = new AddAddressRequest(
            IsPrimary: false,
            Street: "Second St",
            ExternalNumber: "45",
            InternalNumber: null,
            BetweenStreet1: null,
            BetweenStreet2: null,
            Neighbourhood: "North",
            City: "Quito",
            StateOrProvince: "Pichincha",
            PostalCode: "EC17002",
            GoogleMapsUrl: null,
            Latitude: "0",
            Longitude: "0",
            CustomerId: 2,
            CountryId: 1,
            AddressTypeId: 1
        );

        var response = await _client.PostAsJsonAsync("/api/v2/Address", request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var created = await response.Content.ReadFromJsonAsync<AddressResponse>();
        created.Should().NotBeNull();
        created.Street.Should().Be("Second St");
        created.CustomerId.Should().Be(2);
    }

    [Fact]
    public async Task GetById_Returns_404_WhenNotFound()
    {
        var response = await _client.GetAsync("/api/v2/Address/999999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
