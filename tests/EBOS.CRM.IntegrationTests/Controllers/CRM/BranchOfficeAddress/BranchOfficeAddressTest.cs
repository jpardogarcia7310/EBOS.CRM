using System.Net;
using System.Net.Http.Json;
using EBOS.CRM.Contracts.Requests.CRM.Address;
using EBOS.CRM.Contracts.Requests.CRM.BranchOffice;
using EBOS.CRM.Contracts.Requests.CRM.BranchOfficeAddress;
using EBOS.CRM.Contracts.Requests.CRM.CorporateCustomer;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.IntegrationTests.Infrastructure;
using EBOS.CRM.IntegrationTests.TestUtils;
using FluentAssertions;

namespace EBOS.CRM.IntegrationTests.Controllers.CRM.BranchOfficeAddress;

public class BranchOfficeAddressTest(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "BranchOfficeAddress");
    private readonly string _addressVersion = ApiVersionHelper.GetLatestVersion(factory, "Address");
    private readonly string _branchOfficeVersion = ApiVersionHelper.GetLatestVersion(factory, "BranchOffice");
    private readonly string _corporateVersion = ApiVersionHelper.GetLatestVersion(factory, "CorporateCustomer");
    private readonly string _statusVersion = ApiVersionHelper.GetLatestVersion(factory, "Status");
    private readonly string _countryVersion = ApiVersionHelper.GetLatestVersion(factory, "Country");
    private readonly string _addressTypeVersion = ApiVersionHelper.GetLatestVersion(factory, "AddressType");

    [Fact]
    public async Task GetAll_Returns_ListOfItems()
    {
        var response = await _client.GetAsync($"/api/v{_version}/BranchOfficeAddress");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var items = await response.Content.ReadItemsAsync<BranchOfficeAddressResponse>();
        items.Should().NotBeNull();
    }

    [Fact]
    public async Task GetById_Returns_404_WhenNotFound()
    {
        var response = await _client.GetAsync($"/api/v{_version}/BranchOfficeAddress/999999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Add_Update_Delete_Works_For_BranchOfficeAddress()
    {
        var branchOfficeId = await CreateBranchOfficeAsync();
        var addressId = await CreateAddressAsync();

        var addRequest = new AddBranchOfficeAddressRequest(
            TenantId: 1,
            BranchOfficeId: branchOfficeId,
            AddressId: addressId,
            IsPrimary: true,
            ValidFrom: DateTime.UtcNow.Date,
            ValidTo: null,
            IsCurrent: true);

        var addResponse = await _client.PostAsJsonAsync($"/api/v{_version}/BranchOfficeAddress", addRequest);
        addResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var created = await addResponse.Content.ReadFromJsonAsync<BranchOfficeAddressResponse>();
        created.Should().NotBeNull();

        var updateRequest = new UpdateBranchOfficeAddressRequest(
            TenantId: 1,
            BranchOfficeId: branchOfficeId,
            AddressId: addressId,
            IsPrimary: false,
            ValidFrom: DateTime.UtcNow.Date.AddDays(-1),
            ValidTo: DateTime.UtcNow.Date.AddDays(10),
            IsCurrent: true);

        var updateResponse =
            await _client.PutAsJsonAsync($"/api/v{_version}/BranchOfficeAddress/{created.Id}", updateRequest);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var deleteResponse = await _client.DeleteAsync($"/api/v{_version}/BranchOfficeAddress/{created.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    private async Task<long> CreateBranchOfficeAsync()
    {
        var corporateCustomerId = await CreateCorporateCustomerAsync();

        var request = new AddBranchOfficeRequest(
            TenantId: 1,
            Name: "Sucursal Norte",
            PhoneNumber: "34911000333",
            CorporateCustomerId: corporateCustomerId);

        var response = await _client.PostAsJsonAsync($"/api/v{_branchOfficeVersion}/BranchOffice", request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var created = await response.Content.ReadFromJsonAsync<BranchOfficeResponse>();
        created.Should().NotBeNull();
        return created.Id;
    }

    private async Task<long> CreateCorporateCustomerAsync()
    {
        var statusId = await LookupHelper.GetStatusIdAsync(_client, _statusVersion);

        var request = new AddCorporateCustomerRequest(
            TenantId: 1,
            Code: $"CORP-{Guid.NewGuid():N}".Substring(0, 12),
            Email: $"corp{Guid.NewGuid():N}@example.com",
            Phone: "34911000111",
            StatusId: statusId,
            LegalName: "Contoso S.A.",
            TaxIdentification: "B12345678");

        var response = await _client.PostAsJsonAsync($"/api/v{_corporateVersion}/CorporateCustomer", request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var created = await response.Content.ReadFromJsonAsync<CorporateCustomerResponse>();
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






