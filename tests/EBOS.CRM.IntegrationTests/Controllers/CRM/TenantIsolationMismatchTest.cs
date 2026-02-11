using System.Net;
using System.Net.Http.Json;
using EBOS.CRM.Application.Contracts.Requests.CRM.Address;
using EBOS.CRM.Application.Contracts.Requests.CRM.BankInformation;
using EBOS.CRM.Application.Contracts.Requests.CRM.BranchOffice;
using EBOS.CRM.Application.Contracts.Requests.CRM.BranchOfficeAddress;
using EBOS.CRM.Application.Contracts.Requests.CRM.CorporateCustomer;
using EBOS.CRM.Application.Contracts.Requests.CRM.CreditAccount;
using EBOS.CRM.Application.Contracts.Requests.CRM.CreditTransaction;
using EBOS.CRM.Application.Contracts.Requests.CRM.Customer;
using EBOS.CRM.Application.Contracts.Requests.CRM.CustomerAddress;
using EBOS.CRM.Application.Contracts.Requests.CRM.IndividualCustomer;
using EBOS.CRM.Application.Contracts.Requests.CRM.TaxInformation;
using EBOS.CRM.Application.Contracts.Requests.CRM.TaxInformationAddress;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.IntegrationTests.Infrastructure;
using EBOS.CRM.IntegrationTests.TestUtils;
using FluentAssertions;

namespace EBOS.CRM.Api.IntegrationTests.Controllers.CRM;

public class TenantIsolationMismatchTest(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory = factory;

    [Fact]
    public async Task AddCustomer_Returns_400_When_Tenant_Mismatched()
    {
        var client = HttpClientFactory.CreateClientWithTenant(_factory, 1);
        var version = ApiVersionHelper.GetLatestVersion(_factory, "Customer");
        var statusVersion = ApiVersionHelper.GetLatestVersion(_factory, "Status");
        var statusId = await LookupHelper.GetStatusIdAsync(client, statusVersion);

        var request = new AddCustomerRequest(
            TenantId: 2,
            Code: $"CUST-{Guid.NewGuid():N}".Substring(0, 12),
            Email: $"user{Guid.NewGuid():N}@example.com",
            Phone: "600123456",
            StatusId: statusId);

        var response = await client.PostAsJsonAsync($"/api/v{version}/Customer", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task PatchCustomer_Returns_405_When_Not_Supported()
    {
        var client = HttpClientFactory.CreateClientWithTenant(_factory, 1);
        var version = ApiVersionHelper.GetLatestVersion(_factory, "Customer");
        var statusVersion = ApiVersionHelper.GetLatestVersion(_factory, "Status");
        var statusId = await LookupHelper.GetStatusIdAsync(client, statusVersion);

        var addRequest = new AddCustomerRequest(
            TenantId: 1,
            Code: $"CUST-{Guid.NewGuid():N}".Substring(0, 12),
            Email: $"user{Guid.NewGuid():N}@example.com",
            Phone: "600123456",
            StatusId: statusId);

        var addResponse = await client.PostAsJsonAsync($"/api/v{version}/Customer", addRequest);
        addResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var created = await addResponse.Content.ReadFromJsonAsync<CustomerResponse>();
        created.Should().NotBeNull();

        var patchRequest = new PatchCustomerRequest(
            TenantId: 2,
            Code: "PATCH",
            Email: null,
            Phone: null,
            StatusId: null);

        var request = new HttpRequestMessage(HttpMethod.Patch, $"/api/v{version}/Customer/{created!.Id}")
        {
            Content = JsonContent.Create(patchRequest)
        };

        var response = await client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.MethodNotAllowed);
    }

    [Fact]
    public async Task UpdateCustomer_Returns_400_When_Tenant_Mismatched()
    {
        var client = HttpClientFactory.CreateClientWithTenant(_factory, 1);
        var version = ApiVersionHelper.GetLatestVersion(_factory, "Customer");
        var statusVersion = ApiVersionHelper.GetLatestVersion(_factory, "Status");
        var statusId = await LookupHelper.GetStatusIdAsync(client, statusVersion);

        var addRequest = new AddCustomerRequest(
            TenantId: 1,
            Code: $"CUST-{Guid.NewGuid():N}".Substring(0, 12),
            Email: $"user{Guid.NewGuid():N}@example.com",
            Phone: "600123456",
            StatusId: statusId);

        var addResponse = await client.PostAsJsonAsync($"/api/v{version}/Customer", addRequest);
        addResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var created = await addResponse.Content.ReadFromJsonAsync<CustomerResponse>();
        created.Should().NotBeNull();

        var updateRequest = new UpdateCustomerRequest(
            Id: created!.Id,
            TenantId: 2,
            Code: $"CUST-{Guid.NewGuid():N}".Substring(0, 12),
            Email: $"user{Guid.NewGuid():N}@example.com",
            Phone: "600999000",
            StatusId: statusId);

        var updateResponse = await client.PutAsJsonAsync($"/api/v{version}/Customer/{created.Id}", updateRequest);

        updateResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AddAddress_Returns_400_When_Tenant_Mismatched()
    {
        var client = HttpClientFactory.CreateClientWithTenant(_factory, 1);
        var version = ApiVersionHelper.GetLatestVersion(_factory, "Address");
        var countryVersion = ApiVersionHelper.GetLatestVersion(_factory, "Country");
        var addressTypeVersion = ApiVersionHelper.GetLatestVersion(_factory, "AddressType");
        var countryId = await LookupHelper.GetCountryIdAsync(client, countryVersion);
        var addressTypeId = await LookupHelper.GetAddressTypeIdAsync(client, addressTypeVersion);

        var request = new AddAddressRequest(
            TenantId: 2,
            Street: "Main St",
            ExternalNumber: "123",
            InternalNumber: null,
            BetweenStreet1: null,
            BetweenStreet2: null,
            Neighbourhood: "Center",
            City: "Quito",
            StateOrProvince: "Pichincha",
            PostalCode: "EC17001",
            GoogleMapsUrl: "https://maps.example.com/q",
            Latitude: "0",
            Longitude: "0",
            CountryId: countryId,
            AddressTypeId: addressTypeId);

        var response = await client.PostAsJsonAsync($"/api/v{version}/Address", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateAddress_Returns_400_When_Tenant_Mismatched()
    {
        var client = HttpClientFactory.CreateClientWithTenant(_factory, 1);
        var version = ApiVersionHelper.GetLatestVersion(_factory, "Address");
        var countryVersion = ApiVersionHelper.GetLatestVersion(_factory, "Country");
        var addressTypeVersion = ApiVersionHelper.GetLatestVersion(_factory, "AddressType");
        var countryId = await LookupHelper.GetCountryIdAsync(client, countryVersion);
        var addressTypeId = await LookupHelper.GetAddressTypeIdAsync(client, addressTypeVersion);

        var addRequest = new AddAddressRequest(
            TenantId: 1,
            Street: "Main St",
            ExternalNumber: "123",
            InternalNumber: null,
            BetweenStreet1: null,
            BetweenStreet2: null,
            Neighbourhood: "Center",
            City: "Quito",
            StateOrProvince: "Pichincha",
            PostalCode: "EC17001",
            GoogleMapsUrl: "https://maps.example.com/q",
            Latitude: "0",
            Longitude: "0",
            CountryId: countryId,
            AddressTypeId: addressTypeId);
        var addResponse = await client.PostAsJsonAsync($"/api/v{version}/Address", addRequest);
        addResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var address = await addResponse.Content.ReadFromJsonAsync<AddressResponse>();
        address.Should().NotBeNull();

        var updateRequest = new UpdateAddressRequest(
            TenantId: 2,
            Street: "Updated",
            ExternalNumber: "999",
            InternalNumber: null,
            BetweenStreet1: null,
            BetweenStreet2: null,
            Neighbourhood: "Center",
            City: "Quito",
            StateOrProvince: "Pichincha",
            PostalCode: "EC17001",
            GoogleMapsUrl: "https://maps.example.com/q",
            Latitude: "0",
            Longitude: "0",
            CountryId: countryId,
            AddressTypeId: addressTypeId);

        var response = await client.PutAsJsonAsync($"/api/v{version}/Address/{address!.Id}", updateRequest);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AddCorporateCustomer_Returns_400_When_Tenant_Mismatched()
    {
        var client = HttpClientFactory.CreateClientWithTenant(_factory, 1);
        var version = ApiVersionHelper.GetLatestVersion(_factory, "CorporateCustomer");
        var statusVersion = ApiVersionHelper.GetLatestVersion(_factory, "Status");
        var statusId = await LookupHelper.GetStatusIdAsync(client, statusVersion);

        var request = new AddCorporateCustomerRequest(
            TenantId: 2,
            Code: $"CC-{Guid.NewGuid():N}".Substring(0, 10),
            Email: $"corp{Guid.NewGuid():N}@example.com",
            Phone: "600123456",
            StatusId: statusId,
            LegalName: "Corp SA",
            TaxIdentification: "TAX999");

        var response = await client.PostAsJsonAsync($"/api/v{version}/CorporateCustomer", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateCorporateCustomer_Returns_400_When_Tenant_Mismatched()
    {
        var client = HttpClientFactory.CreateClientWithTenant(_factory, 1);
        var version = ApiVersionHelper.GetLatestVersion(_factory, "CorporateCustomer");
        var statusVersion = ApiVersionHelper.GetLatestVersion(_factory, "Status");
        var statusId = await LookupHelper.GetStatusIdAsync(client, statusVersion);

        var addRequest = new AddCorporateCustomerRequest(
            TenantId: 1,
            Code: $"CC-{Guid.NewGuid():N}".Substring(0, 10),
            Email: $"corp{Guid.NewGuid():N}@example.com",
            Phone: "600123456",
            StatusId: statusId,
            LegalName: "Corp SA",
            TaxIdentification: "TAX999");
        var addResponse = await client.PostAsJsonAsync($"/api/v{version}/CorporateCustomer", addRequest);
        addResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var corp = await addResponse.Content.ReadFromJsonAsync<CorporateCustomerResponse>();
        corp.Should().NotBeNull();

        var updateRequest = new UpdateCorporateCustomerRequest(
            TenantId: 2,
            Code: $"CC-{Guid.NewGuid():N}".Substring(0, 10),
            Email: $"corp{Guid.NewGuid():N}@example.com",
            Phone: "600999000",
            StatusId: statusId,
            LegalName: "Corp SA",
            TaxIdentification: "TAX999");

        var response = await client.PutAsJsonAsync($"/api/v{version}/CorporateCustomer/{corp!.Id}", updateRequest);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AddIndividualCustomer_Returns_400_When_Tenant_Mismatched()
    {
        var client = HttpClientFactory.CreateClientWithTenant(_factory, 1);
        var version = ApiVersionHelper.GetLatestVersion(_factory, "IndividualCustomer");
        var statusVersion = ApiVersionHelper.GetLatestVersion(_factory, "Status");
        var idTypeVersion = ApiVersionHelper.GetLatestVersion(_factory, "IdentificationType");
        var statusId = await LookupHelper.GetStatusIdAsync(client, statusVersion);
        var idTypeId = await LookupHelper.GetIdentificationTypeIdAsync(client, idTypeVersion);

        var request = new AddIndividualCustomerRequest(
            TenantId: 2,
            Code: $"IC-{Guid.NewGuid():N}".Substring(0, 10),
            Email: $"person{Guid.NewGuid():N}@example.com",
            Phone: "600123456",
            StatusId: statusId,
            FirstName: "Ana",
            LastName: "Perez",
            BirthDate: new DateTime(1990, 1, 1),
            IdentificationNumber: "ID999",
            IdentificationTypeId: idTypeId);

        var response = await client.PostAsJsonAsync($"/api/v{version}/IndividualCustomer", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateIndividualCustomer_Returns_400_When_Tenant_Mismatched()
    {
        var client = HttpClientFactory.CreateClientWithTenant(_factory, 1);
        var version = ApiVersionHelper.GetLatestVersion(_factory, "IndividualCustomer");
        var statusVersion = ApiVersionHelper.GetLatestVersion(_factory, "Status");
        var idTypeVersion = ApiVersionHelper.GetLatestVersion(_factory, "IdentificationType");
        var statusId = await LookupHelper.GetStatusIdAsync(client, statusVersion);
        var idTypeId = await LookupHelper.GetIdentificationTypeIdAsync(client, idTypeVersion);

        var addRequest = new AddIndividualCustomerRequest(
            TenantId: 1,
            Code: $"IC-{Guid.NewGuid():N}".Substring(0, 10),
            Email: $"person{Guid.NewGuid():N}@example.com",
            Phone: "600123456",
            StatusId: statusId,
            FirstName: "Ana",
            LastName: "Perez",
            BirthDate: new DateTime(1990, 1, 1),
            IdentificationNumber: "ID999",
            IdentificationTypeId: idTypeId);
        var addResponse = await client.PostAsJsonAsync($"/api/v{version}/IndividualCustomer", addRequest);
        addResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var person = await addResponse.Content.ReadFromJsonAsync<IndividualCustomerResponse>();
        person.Should().NotBeNull();

        var updateRequest = new UpdateIndividualCustomerRequest(
            TenantId: 2,
            Code: $"IC-{Guid.NewGuid():N}".Substring(0, 10),
            Email: $"person{Guid.NewGuid():N}@example.com",
            Phone: "600999000",
            StatusId: statusId,
            FirstName: "Ana",
            LastName: "Perez",
            BirthDate: new DateTime(1990, 1, 1),
            IdentificationNumber: "ID999",
            IdentificationTypeId: idTypeId);

        var response = await client.PutAsJsonAsync($"/api/v{version}/IndividualCustomer/{person!.Id}", updateRequest);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AddBranchOffice_Returns_400_When_Tenant_Mismatched()
    {
        var client = HttpClientFactory.CreateClientWithTenant(_factory, 1);
        var corpVersion = ApiVersionHelper.GetLatestVersion(_factory, "CorporateCustomer");
        var branchVersion = ApiVersionHelper.GetLatestVersion(_factory, "BranchOffice");
        var statusVersion = ApiVersionHelper.GetLatestVersion(_factory, "Status");
        var statusId = await LookupHelper.GetStatusIdAsync(client, statusVersion);

        var corpRequest = new AddCorporateCustomerRequest(
            TenantId: 1,
            Code: $"CC-{Guid.NewGuid():N}".Substring(0, 10),
            Email: $"corp{Guid.NewGuid():N}@example.com",
            Phone: "600123456",
            StatusId: statusId,
            LegalName: "Corp SA",
            TaxIdentification: "TAX999");

        var corpResponse = await client.PostAsJsonAsync($"/api/v{corpVersion}/CorporateCustomer", corpRequest);
        corpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var corp = await corpResponse.Content.ReadFromJsonAsync<CorporateCustomerResponse>();
        corp.Should().NotBeNull();

        var request = new AddBranchOfficeRequest(
            TenantId: 2,
            Name: "Branch A",
            PhoneNumber: "123",
            CorporateCustomerId: corp!.Id);

        var response = await client.PostAsJsonAsync($"/api/v{branchVersion}/BranchOffice", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateBranchOffice_Returns_400_When_Tenant_Mismatched()
    {
        var client = HttpClientFactory.CreateClientWithTenant(_factory, 1);
        var corpVersion = ApiVersionHelper.GetLatestVersion(_factory, "CorporateCustomer");
        var branchVersion = ApiVersionHelper.GetLatestVersion(_factory, "BranchOffice");
        var statusVersion = ApiVersionHelper.GetLatestVersion(_factory, "Status");
        var statusId = await LookupHelper.GetStatusIdAsync(client, statusVersion);

        var corpRequest = new AddCorporateCustomerRequest(
            TenantId: 1,
            Code: $"CC-{Guid.NewGuid():N}".Substring(0, 10),
            Email: $"corp{Guid.NewGuid():N}@example.com",
            Phone: "600123456",
            StatusId: statusId,
            LegalName: "Corp SA",
            TaxIdentification: "TAX999");
        var corpResponse = await client.PostAsJsonAsync($"/api/v{corpVersion}/CorporateCustomer", corpRequest);
        corpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var corp = await corpResponse.Content.ReadFromJsonAsync<CorporateCustomerResponse>();
        corp.Should().NotBeNull();

        var branchRequest = new AddBranchOfficeRequest(
            TenantId: 1,
            Name: "Branch A",
            PhoneNumber: "123",
            CorporateCustomerId: corp!.Id);
        var branchResponse = await client.PostAsJsonAsync($"/api/v{branchVersion}/BranchOffice", branchRequest);
        branchResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var branch = await branchResponse.Content.ReadFromJsonAsync<BranchOfficeResponse>();
        branch.Should().NotBeNull();

        var updateRequest = new UpdateBranchOfficeRequest(
            Id: branch!.Id,
            TenantId: 2,
            Name: "Branch B",
            PhoneNumber: "999",
            CorporateCustomerId: corp.Id);

        var response = await client.PutAsJsonAsync($"/api/v{branchVersion}/BranchOffice/{branch.Id}", updateRequest);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task PatchBranchOffice_Returns_405_When_Not_Supported()
    {
        var client = HttpClientFactory.CreateClientWithTenant(_factory, 1);
        var corpVersion = ApiVersionHelper.GetLatestVersion(_factory, "CorporateCustomer");
        var branchVersion = ApiVersionHelper.GetLatestVersion(_factory, "BranchOffice");
        var statusVersion = ApiVersionHelper.GetLatestVersion(_factory, "Status");
        var statusId = await LookupHelper.GetStatusIdAsync(client, statusVersion);

        var corpRequest = new AddCorporateCustomerRequest(
            TenantId: 1,
            Code: $"CC-{Guid.NewGuid():N}".Substring(0, 10),
            Email: $"corp{Guid.NewGuid():N}@example.com",
            Phone: "600123456",
            StatusId: statusId,
            LegalName: "Corp SA",
            TaxIdentification: "TAX999");

        var corpResponse = await client.PostAsJsonAsync($"/api/v{corpVersion}/CorporateCustomer", corpRequest);
        corpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var corp = await corpResponse.Content.ReadFromJsonAsync<CorporateCustomerResponse>();
        corp.Should().NotBeNull();

        var branchRequest = new AddBranchOfficeRequest(
            TenantId: 1,
            Name: "Branch A",
            PhoneNumber: "123",
            CorporateCustomerId: corp!.Id);
        var branchResponse = await client.PostAsJsonAsync($"/api/v{branchVersion}/BranchOffice", branchRequest);
        branchResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var branch = await branchResponse.Content.ReadFromJsonAsync<BranchOfficeResponse>();
        branch.Should().NotBeNull();

        var patchRequest = new PatchBranchOfficeRequest(
            TenantId: 2,
            Name: "Patch",
            PhoneNumber: null,
            CorporateCustomerId: null);

        var request = new HttpRequestMessage(HttpMethod.Patch, $"/api/v{branchVersion}/BranchOffice/{branch!.Id}")
        {
            Content = JsonContent.Create(patchRequest)
        };

        var response = await client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.MethodNotAllowed);
    }

    [Fact]
    public async Task AddBranchOfficeAddress_Returns_400_When_Tenant_Mismatched()
    {
        var client = HttpClientFactory.CreateClientWithTenant(_factory, 1);
        var branchVersion = ApiVersionHelper.GetLatestVersion(_factory, "BranchOffice");
        var addressVersion = ApiVersionHelper.GetLatestVersion(_factory, "Address");
        var boaVersion = ApiVersionHelper.GetLatestVersion(_factory, "BranchOfficeAddress");
        var countryVersion = ApiVersionHelper.GetLatestVersion(_factory, "Country");
        var addressTypeVersion = ApiVersionHelper.GetLatestVersion(_factory, "AddressType");
        var statusVersion = ApiVersionHelper.GetLatestVersion(_factory, "Status");

        var statusId = await LookupHelper.GetStatusIdAsync(client, statusVersion);
        var countryId = await LookupHelper.GetCountryIdAsync(client, countryVersion);
        var addressTypeId = await LookupHelper.GetAddressTypeIdAsync(client, addressTypeVersion);

        var corpRequest = new AddCorporateCustomerRequest(
            TenantId: 1,
            Code: $"CC-{Guid.NewGuid():N}".Substring(0, 10),
            Email: $"corp{Guid.NewGuid():N}@example.com",
            Phone: "600123456",
            StatusId: statusId,
            LegalName: "Corp SA",
            TaxIdentification: "TAX999");

        var corpVersion = ApiVersionHelper.GetLatestVersion(_factory, "CorporateCustomer");
        var corpResponse = await client.PostAsJsonAsync($"/api/v{corpVersion}/CorporateCustomer", corpRequest);
        corpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var corp = await corpResponse.Content.ReadFromJsonAsync<CorporateCustomerResponse>();
        corp.Should().NotBeNull();

        var branchRequest = new AddBranchOfficeRequest(
            TenantId: 1,
            Name: "Branch A",
            PhoneNumber: "123",
            CorporateCustomerId: corp!.Id);
        var branchResponse = await client.PostAsJsonAsync($"/api/v{branchVersion}/BranchOffice", branchRequest);
        branchResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var branch = await branchResponse.Content.ReadFromJsonAsync<BranchOfficeResponse>();
        branch.Should().NotBeNull();

        var addressRequest = new AddAddressRequest(
            TenantId: 1,
            Street: "Main St",
            ExternalNumber: "123",
            InternalNumber: null,
            BetweenStreet1: null,
            BetweenStreet2: null,
            Neighbourhood: "Center",
            City: "Quito",
            StateOrProvince: "Pichincha",
            PostalCode: "EC17001",
            GoogleMapsUrl: "https://maps.example.com/q",
            Latitude: "0",
            Longitude: "0",
            CountryId: countryId,
            AddressTypeId: addressTypeId);
        var addressResponse = await client.PostAsJsonAsync($"/api/v{addressVersion}/Address", addressRequest);
        addressResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var address = await addressResponse.Content.ReadFromJsonAsync<AddressResponse>();
        address.Should().NotBeNull();

        var request = new AddBranchOfficeAddressRequest(
            TenantId: 2,
            BranchOfficeId: branch!.Id,
            AddressId: address!.Id,
            IsPrimary: true,
            ValidFrom: DateTime.UtcNow.Date,
            ValidTo: null,
            IsCurrent: true);

        var response = await client.PostAsJsonAsync($"/api/v{boaVersion}/BranchOfficeAddress", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateBranchOfficeAddress_Returns_400_When_Tenant_Mismatched()
    {
        var client = HttpClientFactory.CreateClientWithTenant(_factory, 1);
        var branchVersion = ApiVersionHelper.GetLatestVersion(_factory, "BranchOffice");
        var addressVersion = ApiVersionHelper.GetLatestVersion(_factory, "Address");
        var boaVersion = ApiVersionHelper.GetLatestVersion(_factory, "BranchOfficeAddress");
        var countryVersion = ApiVersionHelper.GetLatestVersion(_factory, "Country");
        var addressTypeVersion = ApiVersionHelper.GetLatestVersion(_factory, "AddressType");
        var statusVersion = ApiVersionHelper.GetLatestVersion(_factory, "Status");
        var corpVersion = ApiVersionHelper.GetLatestVersion(_factory, "CorporateCustomer");

        var statusId = await LookupHelper.GetStatusIdAsync(client, statusVersion);
        var countryId = await LookupHelper.GetCountryIdAsync(client, countryVersion);
        var addressTypeId = await LookupHelper.GetAddressTypeIdAsync(client, addressTypeVersion);

        var corpRequest = new AddCorporateCustomerRequest(
            TenantId: 1,
            Code: $"CC-{Guid.NewGuid():N}".Substring(0, 10),
            Email: $"corp{Guid.NewGuid():N}@example.com",
            Phone: "600123456",
            StatusId: statusId,
            LegalName: "Corp SA",
            TaxIdentification: "TAX999");
        var corpResponse = await client.PostAsJsonAsync($"/api/v{corpVersion}/CorporateCustomer", corpRequest);
        corpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var corp = await corpResponse.Content.ReadFromJsonAsync<CorporateCustomerResponse>();
        corp.Should().NotBeNull();

        var branchRequest = new AddBranchOfficeRequest(
            TenantId: 1,
            Name: "Branch A",
            PhoneNumber: "123",
            CorporateCustomerId: corp!.Id);
        var branchResponse = await client.PostAsJsonAsync($"/api/v{branchVersion}/BranchOffice", branchRequest);
        branchResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var branch = await branchResponse.Content.ReadFromJsonAsync<BranchOfficeResponse>();
        branch.Should().NotBeNull();

        var addressRequest = new AddAddressRequest(
            TenantId: 1,
            Street: "Main St",
            ExternalNumber: "123",
            InternalNumber: null,
            BetweenStreet1: null,
            BetweenStreet2: null,
            Neighbourhood: "Center",
            City: "Quito",
            StateOrProvince: "Pichincha",
            PostalCode: "EC17001",
            GoogleMapsUrl: "https://maps.example.com/q",
            Latitude: "0",
            Longitude: "0",
            CountryId: countryId,
            AddressTypeId: addressTypeId);
        var addressResponse = await client.PostAsJsonAsync($"/api/v{addressVersion}/Address", addressRequest);
        addressResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var address = await addressResponse.Content.ReadFromJsonAsync<AddressResponse>();
        address.Should().NotBeNull();

        var addRequest = new AddBranchOfficeAddressRequest(
            TenantId: 1,
            BranchOfficeId: branch!.Id,
            AddressId: address!.Id,
            IsPrimary: true,
            ValidFrom: DateTime.UtcNow.Date,
            ValidTo: null,
            IsCurrent: true);
        var addResponse = await client.PostAsJsonAsync($"/api/v{boaVersion}/BranchOfficeAddress", addRequest);
        addResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var boa = await addResponse.Content.ReadFromJsonAsync<BranchOfficeAddressResponse>();
        boa.Should().NotBeNull();

        var updateRequest = new UpdateBranchOfficeAddressRequest(
            TenantId: 2,
            BranchOfficeId: branch.Id,
            AddressId: address.Id,
            IsPrimary: true,
            ValidFrom: DateTime.UtcNow.Date,
            ValidTo: null,
            IsCurrent: true);

        var response = await client.PutAsJsonAsync($"/api/v{boaVersion}/BranchOfficeAddress/{boa!.Id}", updateRequest);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AddBankInformation_Returns_400_When_Tenant_Mismatched()
    {
        var client = HttpClientFactory.CreateClientWithTenant(_factory, 1);
        var customerVersion = ApiVersionHelper.GetLatestVersion(_factory, "Customer");
        var bankVersion = ApiVersionHelper.GetLatestVersion(_factory, "BankInformation");
        var statusVersion = ApiVersionHelper.GetLatestVersion(_factory, "Status");
        var statusId = await LookupHelper.GetStatusIdAsync(client, statusVersion);

        var addCustomer = new AddCustomerRequest(
            TenantId: 1,
            Code: $"CUST-{Guid.NewGuid():N}".Substring(0, 12),
            Email: $"user{Guid.NewGuid():N}@example.com",
            Phone: "600123456",
            StatusId: statusId);
        var addCustomerResponse = await client.PostAsJsonAsync($"/api/v{customerVersion}/Customer", addCustomer);
        addCustomerResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var customer = await addCustomerResponse.Content.ReadFromJsonAsync<CustomerResponse>();
        customer.Should().NotBeNull();

        var request = new AddBankInformationRequest(
            TenantId: 2,
            Iban: "ES1200000000000000000000",
            Bic: "BANKESMM",
            BankName: "Bank",
            CustomerId: customer!.Id);

        var response = await client.PostAsJsonAsync($"/api/v{bankVersion}/BankInformation", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateBankInformation_Returns_400_When_Tenant_Mismatched()
    {
        var client = HttpClientFactory.CreateClientWithTenant(_factory, 1);
        var customerVersion = ApiVersionHelper.GetLatestVersion(_factory, "Customer");
        var bankVersion = ApiVersionHelper.GetLatestVersion(_factory, "BankInformation");
        var statusVersion = ApiVersionHelper.GetLatestVersion(_factory, "Status");
        var statusId = await LookupHelper.GetStatusIdAsync(client, statusVersion);

        var addCustomer = new AddCustomerRequest(
            TenantId: 1,
            Code: $"CUST-{Guid.NewGuid():N}".Substring(0, 12),
            Email: $"user{Guid.NewGuid():N}@example.com",
            Phone: "600123456",
            StatusId: statusId);
        var addCustomerResponse = await client.PostAsJsonAsync($"/api/v{customerVersion}/Customer", addCustomer);
        addCustomerResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var customer = await addCustomerResponse.Content.ReadFromJsonAsync<CustomerResponse>();
        customer.Should().NotBeNull();

        var addRequest = new AddBankInformationRequest(
            TenantId: 1,
            Iban: "ES1200000000000000000000",
            Bic: "BANKESMM",
            BankName: "Bank",
            CustomerId: customer!.Id);
        var addResponse = await client.PostAsJsonAsync($"/api/v{bankVersion}/BankInformation", addRequest);
        addResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var bank = await addResponse.Content.ReadFromJsonAsync<BankInformationResponse>();
        bank.Should().NotBeNull();

        var updateRequest = new UpdateBankInformationRequest(
            TenantId: 2,
            Iban: "ES1200000000000000000001",
            Bic: "BANKESMM",
            BankName: "Bank",
            CustomerId: customer.Id);

        var response = await client.PutAsJsonAsync($"/api/v{bankVersion}/BankInformation/{bank!.Id}", updateRequest);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AddCreditAccount_Returns_400_When_Tenant_Mismatched()
    {
        var client = HttpClientFactory.CreateClientWithTenant(_factory, 1);
        var customerVersion = ApiVersionHelper.GetLatestVersion(_factory, "Customer");
        var creditVersion = ApiVersionHelper.GetLatestVersion(_factory, "CreditAccount");
        var statusVersion = ApiVersionHelper.GetLatestVersion(_factory, "Status");
        var statusId = await LookupHelper.GetStatusIdAsync(client, statusVersion);

        var addCustomer = new AddCustomerRequest(
            TenantId: 1,
            Code: $"CUST-{Guid.NewGuid():N}".Substring(0, 12),
            Email: $"user{Guid.NewGuid():N}@example.com",
            Phone: "600123456",
            StatusId: statusId);
        var addCustomerResponse = await client.PostAsJsonAsync($"/api/v{customerVersion}/Customer", addCustomer);
        addCustomerResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var customer = await addCustomerResponse.Content.ReadFromJsonAsync<CustomerResponse>();
        customer.Should().NotBeNull();

        var request = new AddCreditAccountRequest(
            TenantId: 2,
            MaxAmount: 1000m,
            UsedAmount: 0m,
            CustomerId: customer!.Id);

        var response = await client.PostAsJsonAsync($"/api/v{creditVersion}/CreditAccount", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateCreditAccount_Returns_400_When_Tenant_Mismatched()
    {
        var client = HttpClientFactory.CreateClientWithTenant(_factory, 1);
        var customerVersion = ApiVersionHelper.GetLatestVersion(_factory, "Customer");
        var creditVersion = ApiVersionHelper.GetLatestVersion(_factory, "CreditAccount");
        var statusVersion = ApiVersionHelper.GetLatestVersion(_factory, "Status");
        var statusId = await LookupHelper.GetStatusIdAsync(client, statusVersion);

        var addCustomer = new AddCustomerRequest(
            TenantId: 1,
            Code: $"CUST-{Guid.NewGuid():N}".Substring(0, 12),
            Email: $"user{Guid.NewGuid():N}@example.com",
            Phone: "600123456",
            StatusId: statusId);
        var addCustomerResponse = await client.PostAsJsonAsync($"/api/v{customerVersion}/Customer", addCustomer);
        addCustomerResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var customer = await addCustomerResponse.Content.ReadFromJsonAsync<CustomerResponse>();
        customer.Should().NotBeNull();

        var addCredit = new AddCreditAccountRequest(
            TenantId: 1,
            MaxAmount: 1000m,
            UsedAmount: 0m,
            CustomerId: customer!.Id);
        var creditResponse = await client.PostAsJsonAsync($"/api/v{creditVersion}/CreditAccount", addCredit);
        creditResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var creditAccount = await creditResponse.Content.ReadFromJsonAsync<CreditAccountResponse>();
        creditAccount.Should().NotBeNull();

        var updateRequest = new UpdateCreditAccountRequest(
            Id: creditAccount!.Id,
            TenantId: 2,
            MaxAmount: 900m,
            UsedAmount: 10m,
            CustomerId: customer.Id);

        var response = await client.PutAsJsonAsync($"/api/v{creditVersion}/CreditAccount/{creditAccount.Id}", updateRequest);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task PatchCreditAccount_Returns_405_When_Not_Supported()
    {
        var client = HttpClientFactory.CreateClientWithTenant(_factory, 1);
        var customerVersion = ApiVersionHelper.GetLatestVersion(_factory, "Customer");
        var creditVersion = ApiVersionHelper.GetLatestVersion(_factory, "CreditAccount");
        var statusVersion = ApiVersionHelper.GetLatestVersion(_factory, "Status");
        var statusId = await LookupHelper.GetStatusIdAsync(client, statusVersion);

        var addCustomer = new AddCustomerRequest(
            TenantId: 1,
            Code: $"CUST-{Guid.NewGuid():N}".Substring(0, 12),
            Email: $"user{Guid.NewGuid():N}@example.com",
            Phone: "600123456",
            StatusId: statusId);
        var addCustomerResponse = await client.PostAsJsonAsync($"/api/v{customerVersion}/Customer", addCustomer);
        addCustomerResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var customer = await addCustomerResponse.Content.ReadFromJsonAsync<CustomerResponse>();
        customer.Should().NotBeNull();

        var addCredit = new AddCreditAccountRequest(
            TenantId: 1,
            MaxAmount: 1000m,
            UsedAmount: 0m,
            CustomerId: customer!.Id);
        var creditResponse = await client.PostAsJsonAsync($"/api/v{creditVersion}/CreditAccount", addCredit);
        creditResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var creditAccount = await creditResponse.Content.ReadFromJsonAsync<CreditAccountResponse>();
        creditAccount.Should().NotBeNull();

        var patchRequest = new PatchCreditAccountRequest(
            TenantId: 2,
            MaxAmount: 900m,
            UsedAmount: null,
            CustomerId: null);

        var request = new HttpRequestMessage(HttpMethod.Patch, $"/api/v{creditVersion}/CreditAccount/{creditAccount!.Id}")
        {
            Content = JsonContent.Create(patchRequest)
        };

        var response = await client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.MethodNotAllowed);
    }

    [Fact]
    public async Task AddCreditTransaction_Returns_400_When_Tenant_Mismatched()
    {
        var client = HttpClientFactory.CreateClientWithTenant(_factory, 1);
        var customerVersion = ApiVersionHelper.GetLatestVersion(_factory, "Customer");
        var creditAccountVersion = ApiVersionHelper.GetLatestVersion(_factory, "CreditAccount");
        var creditTransactionVersion = ApiVersionHelper.GetLatestVersion(_factory, "CreditTransaction");
        var statusVersion = ApiVersionHelper.GetLatestVersion(_factory, "Status");
        var statusId = await LookupHelper.GetStatusIdAsync(client, statusVersion);

        var addCustomer = new AddCustomerRequest(
            TenantId: 1,
            Code: $"CUST-{Guid.NewGuid():N}".Substring(0, 12),
            Email: $"user{Guid.NewGuid():N}@example.com",
            Phone: "600123456",
            StatusId: statusId);
        var addCustomerResponse = await client.PostAsJsonAsync($"/api/v{customerVersion}/Customer", addCustomer);
        addCustomerResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var customer = await addCustomerResponse.Content.ReadFromJsonAsync<CustomerResponse>();
        customer.Should().NotBeNull();

        var addCredit = new AddCreditAccountRequest(
            TenantId: 1,
            MaxAmount: 1000m,
            UsedAmount: 0m,
            CustomerId: customer!.Id);
        var creditResponse = await client.PostAsJsonAsync($"/api/v{creditAccountVersion}/CreditAccount", addCredit);
        creditResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var creditAccount = await creditResponse.Content.ReadFromJsonAsync<CreditAccountResponse>();
        creditAccount.Should().NotBeNull();

        var request = new AddCreditTransactionRequest(
            TenantId: 2,
            Date: DateTime.UtcNow,
            Amount: 10m,
            Type: "Consumption",
            ExternalReference: "ORD-1",
            Comments: "Test",
            CreditAccountId: creditAccount!.Id);

        var response = await client.PostAsJsonAsync($"/api/v{creditTransactionVersion}/CreditTransaction", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateCreditTransaction_Returns_400_When_Tenant_Mismatched()
    {
        var client = HttpClientFactory.CreateClientWithTenant(_factory, 1);
        var customerVersion = ApiVersionHelper.GetLatestVersion(_factory, "Customer");
        var creditAccountVersion = ApiVersionHelper.GetLatestVersion(_factory, "CreditAccount");
        var creditTransactionVersion = ApiVersionHelper.GetLatestVersion(_factory, "CreditTransaction");
        var statusVersion = ApiVersionHelper.GetLatestVersion(_factory, "Status");
        var statusId = await LookupHelper.GetStatusIdAsync(client, statusVersion);

        var addCustomer = new AddCustomerRequest(
            TenantId: 1,
            Code: $"CUST-{Guid.NewGuid():N}".Substring(0, 12),
            Email: $"user{Guid.NewGuid():N}@example.com",
            Phone: "600123456",
            StatusId: statusId);
        var addCustomerResponse = await client.PostAsJsonAsync($"/api/v{customerVersion}/Customer", addCustomer);
        addCustomerResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var customer = await addCustomerResponse.Content.ReadFromJsonAsync<CustomerResponse>();
        customer.Should().NotBeNull();

        var addCredit = new AddCreditAccountRequest(
            TenantId: 1,
            MaxAmount: 1000m,
            UsedAmount: 0m,
            CustomerId: customer!.Id);
        var creditResponse = await client.PostAsJsonAsync($"/api/v{creditAccountVersion}/CreditAccount", addCredit);
        creditResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var creditAccount = await creditResponse.Content.ReadFromJsonAsync<CreditAccountResponse>();
        creditAccount.Should().NotBeNull();

        var addRequest = new AddCreditTransactionRequest(
            TenantId: 1,
            Date: DateTime.UtcNow,
            Amount: 10m,
            Type: "Consumption",
            ExternalReference: "ORD-1",
            Comments: "Test",
            CreditAccountId: creditAccount!.Id);
        var addResponse = await client.PostAsJsonAsync($"/api/v{creditTransactionVersion}/CreditTransaction", addRequest);
        addResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var transaction = await addResponse.Content.ReadFromJsonAsync<CreditTransactionResponse>();
        transaction.Should().NotBeNull();

        var updateRequest = new UpdateCreditTransactionRequest(
            TenantId: 2,
            Date: DateTime.UtcNow,
            Amount: 20m,
            Type: "Adjustment",
            ExternalReference: "ORD-2",
            Comments: "Update",
            CreditAccountId: creditAccount.Id);

        var response = await client.PutAsJsonAsync($"/api/v{creditTransactionVersion}/CreditTransaction/{transaction!.Id}", updateRequest);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AddCustomerAddress_Returns_400_When_Tenant_Mismatched()
    {
        var client = HttpClientFactory.CreateClientWithTenant(_factory, 1);
        var customerVersion = ApiVersionHelper.GetLatestVersion(_factory, "Customer");
        var addressVersion = ApiVersionHelper.GetLatestVersion(_factory, "Address");
        var customerAddressVersion = ApiVersionHelper.GetLatestVersion(_factory, "CustomerAddress");
        var statusVersion = ApiVersionHelper.GetLatestVersion(_factory, "Status");
        var countryVersion = ApiVersionHelper.GetLatestVersion(_factory, "Country");
        var addressTypeVersion = ApiVersionHelper.GetLatestVersion(_factory, "AddressType");

        var statusId = await LookupHelper.GetStatusIdAsync(client, statusVersion);
        var countryId = await LookupHelper.GetCountryIdAsync(client, countryVersion);
        var addressTypeId = await LookupHelper.GetAddressTypeIdAsync(client, addressTypeVersion);

        var addCustomer = new AddCustomerRequest(
            TenantId: 1,
            Code: $"CUST-{Guid.NewGuid():N}".Substring(0, 12),
            Email: $"user{Guid.NewGuid():N}@example.com",
            Phone: "600123456",
            StatusId: statusId);
        var addCustomerResponse = await client.PostAsJsonAsync($"/api/v{customerVersion}/Customer", addCustomer);
        addCustomerResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var customer = await addCustomerResponse.Content.ReadFromJsonAsync<CustomerResponse>();
        customer.Should().NotBeNull();

        var addAddress = new AddAddressRequest(
            TenantId: 1,
            Street: "Main St",
            ExternalNumber: "123",
            InternalNumber: null,
            BetweenStreet1: null,
            BetweenStreet2: null,
            Neighbourhood: "Center",
            City: "Quito",
            StateOrProvince: "Pichincha",
            PostalCode: "EC17001",
            GoogleMapsUrl: "https://maps.example.com/q",
            Latitude: "0",
            Longitude: "0",
            CountryId: countryId,
            AddressTypeId: addressTypeId);
        var addAddressResponse = await client.PostAsJsonAsync($"/api/v{addressVersion}/Address", addAddress);
        addAddressResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var address = await addAddressResponse.Content.ReadFromJsonAsync<AddressResponse>();
        address.Should().NotBeNull();

        var request = new AddCustomerAddressRequest(
            TenantId: 2,
            CustomerId: customer!.Id,
            AddressId: address!.Id,
            IsPrimary: true,
            ValidFrom: DateTime.UtcNow.Date,
            ValidTo: null,
            IsCurrent: true);

        var response = await client.PostAsJsonAsync($"/api/v{customerAddressVersion}/CustomerAddress", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateCustomerAddress_Returns_400_When_Tenant_Mismatched()
    {
        var client = HttpClientFactory.CreateClientWithTenant(_factory, 1);
        var customerVersion = ApiVersionHelper.GetLatestVersion(_factory, "Customer");
        var addressVersion = ApiVersionHelper.GetLatestVersion(_factory, "Address");
        var customerAddressVersion = ApiVersionHelper.GetLatestVersion(_factory, "CustomerAddress");
        var statusVersion = ApiVersionHelper.GetLatestVersion(_factory, "Status");
        var countryVersion = ApiVersionHelper.GetLatestVersion(_factory, "Country");
        var addressTypeVersion = ApiVersionHelper.GetLatestVersion(_factory, "AddressType");

        var statusId = await LookupHelper.GetStatusIdAsync(client, statusVersion);
        var countryId = await LookupHelper.GetCountryIdAsync(client, countryVersion);
        var addressTypeId = await LookupHelper.GetAddressTypeIdAsync(client, addressTypeVersion);

        var addCustomer = new AddCustomerRequest(
            TenantId: 1,
            Code: $"CUST-{Guid.NewGuid():N}".Substring(0, 12),
            Email: $"user{Guid.NewGuid():N}@example.com",
            Phone: "600123456",
            StatusId: statusId);
        var addCustomerResponse = await client.PostAsJsonAsync($"/api/v{customerVersion}/Customer", addCustomer);
        addCustomerResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var customer = await addCustomerResponse.Content.ReadFromJsonAsync<CustomerResponse>();
        customer.Should().NotBeNull();

        var addAddress = new AddAddressRequest(
            TenantId: 1,
            Street: "Main St",
            ExternalNumber: "123",
            InternalNumber: null,
            BetweenStreet1: null,
            BetweenStreet2: null,
            Neighbourhood: "Center",
            City: "Quito",
            StateOrProvince: "Pichincha",
            PostalCode: "EC17001",
            GoogleMapsUrl: "https://maps.example.com/q",
            Latitude: "0",
            Longitude: "0",
            CountryId: countryId,
            AddressTypeId: addressTypeId);
        var addAddressResponse = await client.PostAsJsonAsync($"/api/v{addressVersion}/Address", addAddress);
        addAddressResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var address = await addAddressResponse.Content.ReadFromJsonAsync<AddressResponse>();
        address.Should().NotBeNull();

        var addRequest = new AddCustomerAddressRequest(
            TenantId: 1,
            CustomerId: customer!.Id,
            AddressId: address!.Id,
            IsPrimary: true,
            ValidFrom: DateTime.UtcNow.Date,
            ValidTo: null,
            IsCurrent: true);
        var addResponse = await client.PostAsJsonAsync($"/api/v{customerAddressVersion}/CustomerAddress", addRequest);
        addResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var customerAddress = await addResponse.Content.ReadFromJsonAsync<CustomerAddressResponse>();
        customerAddress.Should().NotBeNull();

        var updateRequest = new UpdateCustomerAddressRequest(
            TenantId: 2,
            CustomerId: customer.Id,
            AddressId: address.Id,
            IsPrimary: true,
            ValidFrom: DateTime.UtcNow.Date,
            ValidTo: null,
            IsCurrent: true);

        var response = await client.PutAsJsonAsync($"/api/v{customerAddressVersion}/CustomerAddress/{customerAddress!.Id}", updateRequest);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AddTaxInformation_Returns_400_When_Tenant_Mismatched()
    {
        var client = HttpClientFactory.CreateClientWithTenant(_factory, 1);
        var customerVersion = ApiVersionHelper.GetLatestVersion(_factory, "Customer");
        var taxVersion = ApiVersionHelper.GetLatestVersion(_factory, "TaxInformation");
        var statusVersion = ApiVersionHelper.GetLatestVersion(_factory, "Status");
        var statusId = await LookupHelper.GetStatusIdAsync(client, statusVersion);

        var addCustomer = new AddCustomerRequest(
            TenantId: 1,
            Code: $"CUST-{Guid.NewGuid():N}".Substring(0, 12),
            Email: $"user{Guid.NewGuid():N}@example.com",
            Phone: "600123456",
            StatusId: statusId);
        var addCustomerResponse = await client.PostAsJsonAsync($"/api/v{customerVersion}/Customer", addCustomer);
        addCustomerResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var customer = await addCustomerResponse.Content.ReadFromJsonAsync<CustomerResponse>();
        customer.Should().NotBeNull();

        var request = new AddTaxInformationRequest(
            TenantId: 2,
            TaxName: "IVA",
            TaxIdentificationNumber: "TAX123",
            CustomerId: customer!.Id);

        var response = await client.PostAsJsonAsync($"/api/v{taxVersion}/TaxInformation", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task PatchTaxInformation_Returns_405_When_Not_Supported()
    {
        var client = HttpClientFactory.CreateClientWithTenant(_factory, 1);
        var customerVersion = ApiVersionHelper.GetLatestVersion(_factory, "Customer");
        var taxVersion = ApiVersionHelper.GetLatestVersion(_factory, "TaxInformation");
        var statusVersion = ApiVersionHelper.GetLatestVersion(_factory, "Status");
        var statusId = await LookupHelper.GetStatusIdAsync(client, statusVersion);

        var addCustomer = new AddCustomerRequest(
            TenantId: 1,
            Code: $"CUST-{Guid.NewGuid():N}".Substring(0, 12),
            Email: $"user{Guid.NewGuid():N}@example.com",
            Phone: "600123456",
            StatusId: statusId);
        var addCustomerResponse = await client.PostAsJsonAsync($"/api/v{customerVersion}/Customer", addCustomer);
        addCustomerResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var customer = await addCustomerResponse.Content.ReadFromJsonAsync<CustomerResponse>();
        customer.Should().NotBeNull();

        var addTax = new AddTaxInformationRequest(
            TenantId: 1,
            TaxName: "IVA",
            TaxIdentificationNumber: "TAX123",
            CustomerId: customer!.Id);
        var taxResponse = await client.PostAsJsonAsync($"/api/v{taxVersion}/TaxInformation", addTax);
        taxResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var tax = await taxResponse.Content.ReadFromJsonAsync<TaxInformationResponse>();
        tax.Should().NotBeNull();

        var patchRequest = new PatchTaxInformationRequest(
            TenantId: 2,
            TaxName: "IVA2",
            TaxIdentificationNumber: null,
            CustomerId: null);

        var request = new HttpRequestMessage(HttpMethod.Patch, $"/api/v{taxVersion}/TaxInformation/{tax!.Id}")
        {
            Content = JsonContent.Create(patchRequest)
        };

        var response = await client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.MethodNotAllowed);
    }

    [Fact]
    public async Task UpdateTaxInformation_Returns_400_When_Tenant_Mismatched()
    {
        var client = HttpClientFactory.CreateClientWithTenant(_factory, 1);
        var customerVersion = ApiVersionHelper.GetLatestVersion(_factory, "Customer");
        var taxVersion = ApiVersionHelper.GetLatestVersion(_factory, "TaxInformation");
        var statusVersion = ApiVersionHelper.GetLatestVersion(_factory, "Status");
        var statusId = await LookupHelper.GetStatusIdAsync(client, statusVersion);

        var addCustomer = new AddCustomerRequest(
            TenantId: 1,
            Code: $"CUST-{Guid.NewGuid():N}".Substring(0, 12),
            Email: $"user{Guid.NewGuid():N}@example.com",
            Phone: "600123456",
            StatusId: statusId);
        var addCustomerResponse = await client.PostAsJsonAsync($"/api/v{customerVersion}/Customer", addCustomer);
        addCustomerResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var customer = await addCustomerResponse.Content.ReadFromJsonAsync<CustomerResponse>();
        customer.Should().NotBeNull();

        var addTax = new AddTaxInformationRequest(
            TenantId: 1,
            TaxName: "IVA",
            TaxIdentificationNumber: "TAX123",
            CustomerId: customer!.Id);
        var taxResponse = await client.PostAsJsonAsync($"/api/v{taxVersion}/TaxInformation", addTax);
        taxResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var tax = await taxResponse.Content.ReadFromJsonAsync<TaxInformationResponse>();
        tax.Should().NotBeNull();

        var updateRequest = new UpdateTaxInformationRequest(
            Id: tax!.Id,
            TenantId: 2,
            TaxName: "IVA2",
            TaxIdentificationNumber: "TAX124",
            CustomerId: customer.Id);

        var response = await client.PutAsJsonAsync($"/api/v{taxVersion}/TaxInformation/{tax.Id}", updateRequest);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AddTaxInformationAddress_Returns_400_When_Tenant_Mismatched()
    {
        var client = HttpClientFactory.CreateClientWithTenant(_factory, 1);
        var customerVersion = ApiVersionHelper.GetLatestVersion(_factory, "Customer");
        var taxVersion = ApiVersionHelper.GetLatestVersion(_factory, "TaxInformation");
        var taxAddressVersion = ApiVersionHelper.GetLatestVersion(_factory, "TaxInformationAddress");
        var statusVersion = ApiVersionHelper.GetLatestVersion(_factory, "Status");
        var countryVersion = ApiVersionHelper.GetLatestVersion(_factory, "Country");
        var addressTypeVersion = ApiVersionHelper.GetLatestVersion(_factory, "AddressType");

        var statusId = await LookupHelper.GetStatusIdAsync(client, statusVersion);
        var countryId = await LookupHelper.GetCountryIdAsync(client, countryVersion);
        var addressTypeId = await LookupHelper.GetAddressTypeIdAsync(client, addressTypeVersion);

        var addCustomer = new AddCustomerRequest(
            TenantId: 1,
            Code: $"CUST-{Guid.NewGuid():N}".Substring(0, 12),
            Email: $"user{Guid.NewGuid():N}@example.com",
            Phone: "600123456",
            StatusId: statusId);
        var addCustomerResponse = await client.PostAsJsonAsync($"/api/v{customerVersion}/Customer", addCustomer);
        addCustomerResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var customer = await addCustomerResponse.Content.ReadFromJsonAsync<CustomerResponse>();
        customer.Should().NotBeNull();

        var addTax = new AddTaxInformationRequest(
            TenantId: 1,
            TaxName: "IVA",
            TaxIdentificationNumber: "TAX123",
            CustomerId: customer!.Id);
        var taxResponse = await client.PostAsJsonAsync($"/api/v{taxVersion}/TaxInformation", addTax);
        taxResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var tax = await taxResponse.Content.ReadFromJsonAsync<TaxInformationResponse>();
        tax.Should().NotBeNull();

        var addAddress = new AddAddressRequest(
            TenantId: 1,
            Street: "Main St",
            ExternalNumber: "123",
            InternalNumber: null,
            BetweenStreet1: null,
            BetweenStreet2: null,
            Neighbourhood: "Center",
            City: "Quito",
            StateOrProvince: "Pichincha",
            PostalCode: "EC17001",
            GoogleMapsUrl: "https://maps.example.com/q",
            Latitude: "0",
            Longitude: "0",
            CountryId: countryId,
            AddressTypeId: addressTypeId);
        var addressResponse = await client.PostAsJsonAsync($"/api/v{ApiVersionHelper.GetLatestVersion(_factory, "Address")}/Address", addAddress);
        addressResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var address = await addressResponse.Content.ReadFromJsonAsync<AddressResponse>();
        address.Should().NotBeNull();

        var request = new AddTaxInformationAddressRequest(
            TenantId: 2,
            TaxInformationId: tax!.Id,
            AddressId: address!.Id,
            IsPrimary: true,
            ValidFrom: DateTime.UtcNow.Date,
            ValidTo: null,
            IsCurrent: true);

        var response = await client.PostAsJsonAsync($"/api/v{taxAddressVersion}/TaxInformationAddress", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateTaxInformationAddress_Returns_400_When_Tenant_Mismatched()
    {
        var client = HttpClientFactory.CreateClientWithTenant(_factory, 1);
        var customerVersion = ApiVersionHelper.GetLatestVersion(_factory, "Customer");
        var taxVersion = ApiVersionHelper.GetLatestVersion(_factory, "TaxInformation");
        var taxAddressVersion = ApiVersionHelper.GetLatestVersion(_factory, "TaxInformationAddress");
        var statusVersion = ApiVersionHelper.GetLatestVersion(_factory, "Status");
        var countryVersion = ApiVersionHelper.GetLatestVersion(_factory, "Country");
        var addressTypeVersion = ApiVersionHelper.GetLatestVersion(_factory, "AddressType");
        var addressVersion = ApiVersionHelper.GetLatestVersion(_factory, "Address");

        var statusId = await LookupHelper.GetStatusIdAsync(client, statusVersion);
        var countryId = await LookupHelper.GetCountryIdAsync(client, countryVersion);
        var addressTypeId = await LookupHelper.GetAddressTypeIdAsync(client, addressTypeVersion);

        var addCustomer = new AddCustomerRequest(
            TenantId: 1,
            Code: $"CUST-{Guid.NewGuid():N}".Substring(0, 12),
            Email: $"user{Guid.NewGuid():N}@example.com",
            Phone: "600123456",
            StatusId: statusId);
        var addCustomerResponse = await client.PostAsJsonAsync($"/api/v{customerVersion}/Customer", addCustomer);
        addCustomerResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var customer = await addCustomerResponse.Content.ReadFromJsonAsync<CustomerResponse>();
        customer.Should().NotBeNull();

        var addTax = new AddTaxInformationRequest(
            TenantId: 1,
            TaxName: "IVA",
            TaxIdentificationNumber: "TAX123",
            CustomerId: customer!.Id);
        var taxResponse = await client.PostAsJsonAsync($"/api/v{taxVersion}/TaxInformation", addTax);
        taxResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var tax = await taxResponse.Content.ReadFromJsonAsync<TaxInformationResponse>();
        tax.Should().NotBeNull();

        var addAddress = new AddAddressRequest(
            TenantId: 1,
            Street: "Main St",
            ExternalNumber: "123",
            InternalNumber: null,
            BetweenStreet1: null,
            BetweenStreet2: null,
            Neighbourhood: "Center",
            City: "Quito",
            StateOrProvince: "Pichincha",
            PostalCode: "EC17001",
            GoogleMapsUrl: "https://maps.example.com/q",
            Latitude: "0",
            Longitude: "0",
            CountryId: countryId,
            AddressTypeId: addressTypeId);
        var addressResponse = await client.PostAsJsonAsync($"/api/v{addressVersion}/Address", addAddress);
        addressResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var address = await addressResponse.Content.ReadFromJsonAsync<AddressResponse>();
        address.Should().NotBeNull();

        var addRequest = new AddTaxInformationAddressRequest(
            TenantId: 1,
            TaxInformationId: tax!.Id,
            AddressId: address!.Id,
            IsPrimary: true,
            ValidFrom: DateTime.UtcNow.Date,
            ValidTo: null,
            IsCurrent: true);
        var addResponse = await client.PostAsJsonAsync($"/api/v{taxAddressVersion}/TaxInformationAddress", addRequest);
        addResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var taxAddress = await addResponse.Content.ReadFromJsonAsync<TaxInformationAddressResponse>();
        taxAddress.Should().NotBeNull();

        var updateRequest = new UpdateTaxInformationAddressRequest(
            TenantId: 2,
            TaxInformationId: tax.Id,
            AddressId: address.Id,
            IsPrimary: true,
            ValidFrom: DateTime.UtcNow.Date,
            ValidTo: null,
            IsCurrent: true);

        var response = await client.PutAsJsonAsync($"/api/v{taxAddressVersion}/TaxInformationAddress/{taxAddress!.Id}", updateRequest);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
