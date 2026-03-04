using System.Net;
using System.Net.Http.Json;
using EBOS.CRM.Contracts.Requests.CRM.Address;
using EBOS.CRM.Contracts.Requests.CRM.Customer;
using EBOS.CRM.Contracts.Requests.CRM.CustomerAddress;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.IntegrationTests.Infrastructure;
using EBOS.CRM.IntegrationTests.TestUtils;
using FluentAssertions;

namespace EBOS.CRM.IntegrationTests.Controllers.CRM.CustomerAddress;

public class CustomerAddressTest(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "CustomerAddress");
    private readonly string _customerVersion = ApiVersionHelper.GetLatestVersion(factory, "Customer");
    private readonly string _statusVersion = ApiVersionHelper.GetLatestVersion(factory, "Status");
    private readonly string _addressVersion = ApiVersionHelper.GetLatestVersion(factory, "Address");
    private readonly string _countryVersion = ApiVersionHelper.GetLatestVersion(factory, "Country");
    private readonly string _addressTypeVersion = ApiVersionHelper.GetLatestVersion(factory, "AddressType");

    [Fact]
    public async Task GetAll_Returns_ListOfItems()
    {
        var response = await _client.GetAsync($"/api/v{_version}/CustomerAddress");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var items = await response.Content.ReadItemsAsync<CustomerAddressResponse>();
        items.Should().NotBeNull();
    }

    [Fact]
    public async Task GetById_Returns_404_WhenNotFound()
    {
        var response = await _client.GetAsync($"/api/v{_version}/CustomerAddress/999999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Add_Update_Delete_Works_For_CustomerAddress()
    {
        var customerId = await CreateCustomerAsync();
        var addressId = await CreateAddressAsync();

        var addRequest = new AddCustomerAddressRequest(
            TenantId: 1,
            CustomerId: customerId,
            AddressId: addressId,
            IsPrimary: true,
            ValidFrom: DateTime.UtcNow.Date,
            ValidTo: null,
            IsCurrent: true);

        var addResponse = await _client.PostAsJsonAsync($"/api/v{_version}/CustomerAddress", addRequest);
        addResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var created = await addResponse.Content.ReadFromJsonAsync<CustomerAddressResponse>();
        created.Should().NotBeNull();

        var updateRequest = new UpdateCustomerAddressRequest(
            TenantId: 1,
            CustomerId: customerId,
            AddressId: addressId,
            IsPrimary: false,
            ValidFrom: DateTime.UtcNow.Date.AddDays(-1),
            ValidTo: DateTime.UtcNow.Date.AddDays(5),
            IsCurrent: true);

        var updateResponse =
            await _client.PutAsJsonAsync($"/api/v{_version}/CustomerAddress/{created.Id}", updateRequest);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var deleteResponse = await _client.DeleteAsync($"/api/v{_version}/CustomerAddress/{created.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    private async Task<long> CreateCustomerAsync()
    {
        var statusId = await LookupHelper.GetStatusIdAsync(_client, _statusVersion);

        var request = new AddCustomerRequest(
            TenantId: 1,
            Code: $"CUST-{Guid.NewGuid():N}".Substring(0, 12),
            Email: $"user{Guid.NewGuid():N}@example.com",
            Phone: "34600123456",
            StatusId: statusId);

        var response = await _client.PostAsJsonAsync($"/api/v{_customerVersion}/Customer", request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var created = await response.Content.ReadFromJsonAsync<CustomerResponse>();
        created.Should().NotBeNull();
        return created.Id;
    }

    private async Task<long> CreateAddressAsync()
    {
        var countryId = await LookupHelper.GetCountryIdAsync(_client, _countryVersion);
        var addressTypeId = await LookupHelper.GetAddressTypeIdAsync(_client, _addressTypeVersion);

        var request = new AddAddressRequest(
            TenantId: 1,
            Street: "Calle Mayor",
            ExternalNumber: "10",
            InternalNumber: null,
            BetweenStreet1: null,
            BetweenStreet2: null,
            Neighbourhood: "Centro",
            City: "Madrid",
            StateOrProvince: "Madrid",
            PostalCode: "28013",
            GoogleMapsUrl: null,
            Latitude: "40.4168",
            Longitude: "-3.7038",
            CountryId: countryId,
            AddressTypeId: addressTypeId
        );

        var response = await _client.PostAsJsonAsync($"/api/v{_addressVersion}/Address", request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var created = await response.Content.ReadFromJsonAsync<AddressResponse>();
        created.Should().NotBeNull();
        return created.Id;
    }
}






