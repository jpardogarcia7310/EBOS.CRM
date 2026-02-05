using System.Net;
using System.Net.Http.Json;
using EBOS.CRM.Api.IntegrationTests.Infrastructure;
using EBOS.CRM.Api.IntegrationTests.TestUtils;
using EBOS.CRM.Application.Contracts.Requests.CRM.Address;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using FluentAssertions;

namespace EBOS.CRM.Api.IntegrationTests.Controllers.CRM.Address;

public class AddressTest : IClassFixture<InMemoryAddressWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly string _version;

    public AddressTest(InMemoryAddressWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        _version = ApiVersionHelper.GetLatestVersion(factory, "Address");

        if (!factory.Repository.Items.Any())
        {
            factory.Repository.AddAsync(new Domain.Entities.CRM.Address
            {
                Id = 1,
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
                CountryId = 1,
                AddressTypeId = 1
            }).GetAwaiter().GetResult();
        }
    }

    [Fact]
    public async Task GetAll_Returns_ListOfItems()
    {
        var response = await _client.GetAsync($"/api/v{_version}/Address");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var items = await response.Content.ReadItemsAsync<AddressResponse>();
        items.Should().NotBeNull();
        items.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetById_Returns_Address_WhenExists()
    {
        var response = await _client.GetAsync($"/api/v{_version}/Address/1");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var item = await response.Content.ReadFromJsonAsync<AddressResponse>();
        item.Should().NotBeNull();
        item.Street.Should().Be("Main St");
    }

    [Fact]
    public async Task Add_Returns_Address_WhenValid()
    {
        var request = new AddAddressRequest(
            TenantId: 1,
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
            CountryId: 1,
            AddressTypeId: 1
        );

        var response = await _client.PostAsJsonAsync($"/api/v{_version}/Address", request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var created = await response.Content.ReadFromJsonAsync<AddressResponse>();
        created.Should().NotBeNull();
        created.Street.Should().Be("Second St");
    }

    [Fact]
    public async Task Update_Returns_Address_WhenValid()
    {
        var request = new UpdateAddressRequest(
            TenantId: 1,
            Street: "Updated St",
            ExternalNumber: "777",
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

        var response = await _client.PutAsJsonAsync($"/api/v{_version}/Address/1", request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var updated = await response.Content.ReadFromJsonAsync<AddressResponse>();
        updated.Should().NotBeNull();
        updated.Street.Should().Be("Updated St");
    }

    [Fact]
    public async Task Update_Returns_404_WhenNotFound()
    {
        var request = new UpdateAddressRequest(
            TenantId: 1,
            Street: "Updated St",
            ExternalNumber: "777",
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

        var response = await _client.PutAsJsonAsync($"/api/v{_version}/Address/999999", request);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_Returns_OK_WhenExists()
    {
        var response = await _client.DeleteAsync($"/api/v{_version}/Address/1");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var getResponse = await _client.GetAsync($"/api/v{_version}/Address/1");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_Returns_404_WhenNotFound()
    {
        var response = await _client.DeleteAsync($"/api/v{_version}/Address/999999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetById_Returns_404_WhenNotFound()
    {
        var response = await _client.GetAsync($"/api/v{_version}/Address/999999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}










