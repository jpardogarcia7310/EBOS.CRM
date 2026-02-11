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

public class TenantIsolationDeleteMismatchTest(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory = factory;

    [Fact]
    public async Task DeleteCustomer_Returns_404_When_Tenant_Mismatched()
    {
        var (client1, client2) = CreateClients();
        var version = ApiVersionHelper.GetLatestVersion(_factory, "Customer");
        var statusId = await LookupHelper.GetStatusIdAsync(client1, ApiVersionHelper.GetLatestVersion(_factory, "Status"));

        var addResponse = await client1.PostAsJsonAsync($"/api/v{version}/Customer",
            new AddCustomerRequest(1, $"CUST-{Guid.NewGuid():N}".Substring(0, 12), $"user{Guid.NewGuid():N}@example.com", "600123456", statusId));
        addResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var created = await addResponse.Content.ReadFromJsonAsync<CustomerResponse>();
        created.Should().NotBeNull();

        var response = await client2.DeleteAsync($"/api/v{version}/Customer/{created!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteAddress_Returns_404_When_Tenant_Mismatched()
    {
        var (client1, client2) = CreateClients();
        var version = ApiVersionHelper.GetLatestVersion(_factory, "Address");
        var countryId = await LookupHelper.GetCountryIdAsync(client1, ApiVersionHelper.GetLatestVersion(_factory, "Country"));
        var addressTypeId = await LookupHelper.GetAddressTypeIdAsync(client1, ApiVersionHelper.GetLatestVersion(_factory, "AddressType"));

        var addResponse = await client1.PostAsJsonAsync($"/api/v{version}/Address",
            new AddAddressRequest(1, "Main St", "123", null, null, null, "Center", "Quito", "Pichincha", "EC17001",
                "https://maps.example.com/q", "0", "0", countryId, addressTypeId));
        addResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var created = await addResponse.Content.ReadFromJsonAsync<AddressResponse>();
        created.Should().NotBeNull();

        var response = await client2.DeleteAsync($"/api/v{version}/Address/{created!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteCorporateCustomer_Returns_404_When_Tenant_Mismatched()
    {
        var (client1, client2) = CreateClients();
        var version = ApiVersionHelper.GetLatestVersion(_factory, "CorporateCustomer");
        var statusId = await LookupHelper.GetStatusIdAsync(client1, ApiVersionHelper.GetLatestVersion(_factory, "Status"));

        var addResponse = await client1.PostAsJsonAsync($"/api/v{version}/CorporateCustomer",
            new AddCorporateCustomerRequest(1, $"CC-{Guid.NewGuid():N}".Substring(0, 10), $"corp{Guid.NewGuid():N}@example.com", "600123456",
                statusId, "Corp SA", "TAX999"));
        addResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var created = await addResponse.Content.ReadFromJsonAsync<CorporateCustomerResponse>();
        created.Should().NotBeNull();

        var response = await client2.DeleteAsync($"/api/v{version}/CorporateCustomer/{created!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteIndividualCustomer_Returns_404_When_Tenant_Mismatched()
    {
        var (client1, client2) = CreateClients();
        var version = ApiVersionHelper.GetLatestVersion(_factory, "IndividualCustomer");
        var statusId = await LookupHelper.GetStatusIdAsync(client1, ApiVersionHelper.GetLatestVersion(_factory, "Status"));
        var idTypeId = await LookupHelper.GetIdentificationTypeIdAsync(client1, ApiVersionHelper.GetLatestVersion(_factory, "IdentificationType"));

        var addResponse = await client1.PostAsJsonAsync($"/api/v{version}/IndividualCustomer",
            new AddIndividualCustomerRequest(1, $"IC-{Guid.NewGuid():N}".Substring(0, 10), $"person{Guid.NewGuid():N}@example.com", "600123456",
                statusId, "Ana", "Perez", new DateTime(1990, 1, 1), "ID999", idTypeId));
        addResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var created = await addResponse.Content.ReadFromJsonAsync<IndividualCustomerResponse>();
        created.Should().NotBeNull();

        var response = await client2.DeleteAsync($"/api/v{version}/IndividualCustomer/{created!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteBranchOffice_Returns_404_When_Tenant_Mismatched()
    {
        var (client1, client2) = CreateClients();
        var statusId = await LookupHelper.GetStatusIdAsync(client1, ApiVersionHelper.GetLatestVersion(_factory, "Status"));
        var corpVersion = ApiVersionHelper.GetLatestVersion(_factory, "CorporateCustomer");
        var branchVersion = ApiVersionHelper.GetLatestVersion(_factory, "BranchOffice");

        var corpResponse = await client1.PostAsJsonAsync($"/api/v{corpVersion}/CorporateCustomer",
            new AddCorporateCustomerRequest(1, $"CC-{Guid.NewGuid():N}".Substring(0, 10), $"corp{Guid.NewGuid():N}@example.com", "600123456",
                statusId, "Corp SA", "TAX999"));
        corpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var corp = await corpResponse.Content.ReadFromJsonAsync<CorporateCustomerResponse>();
        corp.Should().NotBeNull();

        var addResponse = await client1.PostAsJsonAsync($"/api/v{branchVersion}/BranchOffice",
            new AddBranchOfficeRequest(1, "Branch A", "123", corp!.Id));
        addResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var created = await addResponse.Content.ReadFromJsonAsync<BranchOfficeResponse>();
        created.Should().NotBeNull();

        var response = await client2.DeleteAsync($"/api/v{branchVersion}/BranchOffice/{created!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteBranchOfficeAddress_Returns_404_When_Tenant_Mismatched()
    {
        var (client1, client2) = CreateClients();
        var statusId = await LookupHelper.GetStatusIdAsync(client1, ApiVersionHelper.GetLatestVersion(_factory, "Status"));
        var countryId = await LookupHelper.GetCountryIdAsync(client1, ApiVersionHelper.GetLatestVersion(_factory, "Country"));
        var addressTypeId = await LookupHelper.GetAddressTypeIdAsync(client1, ApiVersionHelper.GetLatestVersion(_factory, "AddressType"));
        var corpVersion = ApiVersionHelper.GetLatestVersion(_factory, "CorporateCustomer");
        var branchVersion = ApiVersionHelper.GetLatestVersion(_factory, "BranchOffice");
        var addressVersion = ApiVersionHelper.GetLatestVersion(_factory, "Address");
        var boaVersion = ApiVersionHelper.GetLatestVersion(_factory, "BranchOfficeAddress");

        var corpResponse = await client1.PostAsJsonAsync($"/api/v{corpVersion}/CorporateCustomer",
            new AddCorporateCustomerRequest(1, $"CC-{Guid.NewGuid():N}".Substring(0, 10), $"corp{Guid.NewGuid():N}@example.com", "600123456",
                statusId, "Corp SA", "TAX999"));
        corpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var corp = await corpResponse.Content.ReadFromJsonAsync<CorporateCustomerResponse>();
        corp.Should().NotBeNull();

        var branchResponse = await client1.PostAsJsonAsync($"/api/v{branchVersion}/BranchOffice",
            new AddBranchOfficeRequest(1, "Branch A", "123", corp!.Id));
        branchResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var branch = await branchResponse.Content.ReadFromJsonAsync<BranchOfficeResponse>();
        branch.Should().NotBeNull();

        var addressResponse = await client1.PostAsJsonAsync($"/api/v{addressVersion}/Address",
            new AddAddressRequest(1, "Main St", "123", null, null, null, "Center", "Quito", "Pichincha", "EC17001",
                "https://maps.example.com/q", "0", "0", countryId, addressTypeId));
        addressResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var address = await addressResponse.Content.ReadFromJsonAsync<AddressResponse>();
        address.Should().NotBeNull();

        var addResponse = await client1.PostAsJsonAsync($"/api/v{boaVersion}/BranchOfficeAddress",
            new AddBranchOfficeAddressRequest(1, branch!.Id, address!.Id, true, DateTime.UtcNow.Date, null, true));
        addResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var created = await addResponse.Content.ReadFromJsonAsync<BranchOfficeAddressResponse>();
        created.Should().NotBeNull();

        var response = await client2.DeleteAsync($"/api/v{boaVersion}/BranchOfficeAddress/{created!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteBankInformation_Returns_404_When_Tenant_Mismatched()
    {
        var (client1, client2) = CreateClients();
        var statusId = await LookupHelper.GetStatusIdAsync(client1, ApiVersionHelper.GetLatestVersion(_factory, "Status"));
        var customerVersion = ApiVersionHelper.GetLatestVersion(_factory, "Customer");
        var bankVersion = ApiVersionHelper.GetLatestVersion(_factory, "BankInformation");

        var customerResponse = await client1.PostAsJsonAsync($"/api/v{customerVersion}/Customer",
            new AddCustomerRequest(1, $"CUST-{Guid.NewGuid():N}".Substring(0, 12), $"user{Guid.NewGuid():N}@example.com", "600123456", statusId));
        customerResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var customer = await customerResponse.Content.ReadFromJsonAsync<CustomerResponse>();
        customer.Should().NotBeNull();

        var addResponse = await client1.PostAsJsonAsync($"/api/v{bankVersion}/BankInformation",
            new AddBankInformationRequest(1, "ES1200000000000000000000", "BANKESMM", "Bank", customer!.Id));
        addResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var created = await addResponse.Content.ReadFromJsonAsync<BankInformationResponse>();
        created.Should().NotBeNull();

        var response = await client2.DeleteAsync($"/api/v{bankVersion}/BankInformation/{created!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteCreditAccount_Returns_404_When_Tenant_Mismatched()
    {
        var (client1, client2) = CreateClients();
        var statusId = await LookupHelper.GetStatusIdAsync(client1, ApiVersionHelper.GetLatestVersion(_factory, "Status"));
        var customerVersion = ApiVersionHelper.GetLatestVersion(_factory, "Customer");
        var creditVersion = ApiVersionHelper.GetLatestVersion(_factory, "CreditAccount");

        var customerResponse = await client1.PostAsJsonAsync($"/api/v{customerVersion}/Customer",
            new AddCustomerRequest(1, $"CUST-{Guid.NewGuid():N}".Substring(0, 12), $"user{Guid.NewGuid():N}@example.com", "600123456", statusId));
        customerResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var customer = await customerResponse.Content.ReadFromJsonAsync<CustomerResponse>();
        customer.Should().NotBeNull();

        var addResponse = await client1.PostAsJsonAsync($"/api/v{creditVersion}/CreditAccount",
            new AddCreditAccountRequest(1, 1000m, 0m, customer!.Id));
        addResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var created = await addResponse.Content.ReadFromJsonAsync<CreditAccountResponse>();
        created.Should().NotBeNull();

        var response = await client2.DeleteAsync($"/api/v{creditVersion}/CreditAccount/{created!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteCreditTransaction_Returns_404_When_Tenant_Mismatched()
    {
        var (client1, client2) = CreateClients();
        var statusId = await LookupHelper.GetStatusIdAsync(client1, ApiVersionHelper.GetLatestVersion(_factory, "Status"));
        var customerVersion = ApiVersionHelper.GetLatestVersion(_factory, "Customer");
        var creditVersion = ApiVersionHelper.GetLatestVersion(_factory, "CreditAccount");
        var transactionVersion = ApiVersionHelper.GetLatestVersion(_factory, "CreditTransaction");

        var customerResponse = await client1.PostAsJsonAsync($"/api/v{customerVersion}/Customer",
            new AddCustomerRequest(1, $"CUST-{Guid.NewGuid():N}".Substring(0, 12), $"user{Guid.NewGuid():N}@example.com", "600123456", statusId));
        customerResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var customer = await customerResponse.Content.ReadFromJsonAsync<CustomerResponse>();
        customer.Should().NotBeNull();

        var creditResponse = await client1.PostAsJsonAsync($"/api/v{creditVersion}/CreditAccount",
            new AddCreditAccountRequest(1, 1000m, 0m, customer!.Id));
        creditResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var credit = await creditResponse.Content.ReadFromJsonAsync<CreditAccountResponse>();
        credit.Should().NotBeNull();

        var addResponse = await client1.PostAsJsonAsync($"/api/v{transactionVersion}/CreditTransaction",
            new AddCreditTransactionRequest(1, DateTime.UtcNow, 10m, "Consumption", "ORD-1", "Test", credit!.Id));
        addResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var created = await addResponse.Content.ReadFromJsonAsync<CreditTransactionResponse>();
        created.Should().NotBeNull();

        var response = await client2.DeleteAsync($"/api/v{transactionVersion}/CreditTransaction/{created!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteCustomerAddress_Returns_404_When_Tenant_Mismatched()
    {
        var (client1, client2) = CreateClients();
        var statusId = await LookupHelper.GetStatusIdAsync(client1, ApiVersionHelper.GetLatestVersion(_factory, "Status"));
        var countryId = await LookupHelper.GetCountryIdAsync(client1, ApiVersionHelper.GetLatestVersion(_factory, "Country"));
        var addressTypeId = await LookupHelper.GetAddressTypeIdAsync(client1, ApiVersionHelper.GetLatestVersion(_factory, "AddressType"));
        var customerVersion = ApiVersionHelper.GetLatestVersion(_factory, "Customer");
        var addressVersion = ApiVersionHelper.GetLatestVersion(_factory, "Address");
        var customerAddressVersion = ApiVersionHelper.GetLatestVersion(_factory, "CustomerAddress");

        var customerResponse = await client1.PostAsJsonAsync($"/api/v{customerVersion}/Customer",
            new AddCustomerRequest(1, $"CUST-{Guid.NewGuid():N}".Substring(0, 12), $"user{Guid.NewGuid():N}@example.com", "600123456", statusId));
        customerResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var customer = await customerResponse.Content.ReadFromJsonAsync<CustomerResponse>();
        customer.Should().NotBeNull();

        var addressResponse = await client1.PostAsJsonAsync($"/api/v{addressVersion}/Address",
            new AddAddressRequest(1, "Main St", "123", null, null, null, "Center", "Quito", "Pichincha", "EC17001",
                "https://maps.example.com/q", "0", "0", countryId, addressTypeId));
        addressResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var address = await addressResponse.Content.ReadFromJsonAsync<AddressResponse>();
        address.Should().NotBeNull();

        var addResponse = await client1.PostAsJsonAsync($"/api/v{customerAddressVersion}/CustomerAddress",
            new AddCustomerAddressRequest(1, customer!.Id, address!.Id, true, DateTime.UtcNow.Date, null, true));
        addResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var created = await addResponse.Content.ReadFromJsonAsync<CustomerAddressResponse>();
        created.Should().NotBeNull();

        var response = await client2.DeleteAsync($"/api/v{customerAddressVersion}/CustomerAddress/{created!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteTaxInformation_Returns_404_When_Tenant_Mismatched()
    {
        var (client1, client2) = CreateClients();
        var statusId = await LookupHelper.GetStatusIdAsync(client1, ApiVersionHelper.GetLatestVersion(_factory, "Status"));
        var customerVersion = ApiVersionHelper.GetLatestVersion(_factory, "Customer");
        var taxVersion = ApiVersionHelper.GetLatestVersion(_factory, "TaxInformation");

        var customerResponse = await client1.PostAsJsonAsync($"/api/v{customerVersion}/Customer",
            new AddCustomerRequest(1, $"CUST-{Guid.NewGuid():N}".Substring(0, 12), $"user{Guid.NewGuid():N}@example.com", "600123456", statusId));
        customerResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var customer = await customerResponse.Content.ReadFromJsonAsync<CustomerResponse>();
        customer.Should().NotBeNull();

        var addResponse = await client1.PostAsJsonAsync($"/api/v{taxVersion}/TaxInformation",
            new AddTaxInformationRequest(1, "IVA", "TAX123", customer!.Id));
        addResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var created = await addResponse.Content.ReadFromJsonAsync<TaxInformationResponse>();
        created.Should().NotBeNull();

        var response = await client2.DeleteAsync($"/api/v{taxVersion}/TaxInformation/{created!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteTaxInformationAddress_Returns_404_When_Tenant_Mismatched()
    {
        var (client1, client2) = CreateClients();
        var statusId = await LookupHelper.GetStatusIdAsync(client1, ApiVersionHelper.GetLatestVersion(_factory, "Status"));
        var countryId = await LookupHelper.GetCountryIdAsync(client1, ApiVersionHelper.GetLatestVersion(_factory, "Country"));
        var addressTypeId = await LookupHelper.GetAddressTypeIdAsync(client1, ApiVersionHelper.GetLatestVersion(_factory, "AddressType"));
        var customerVersion = ApiVersionHelper.GetLatestVersion(_factory, "Customer");
        var addressVersion = ApiVersionHelper.GetLatestVersion(_factory, "Address");
        var taxVersion = ApiVersionHelper.GetLatestVersion(_factory, "TaxInformation");
        var taxAddressVersion = ApiVersionHelper.GetLatestVersion(_factory, "TaxInformationAddress");

        var customerResponse = await client1.PostAsJsonAsync($"/api/v{customerVersion}/Customer",
            new AddCustomerRequest(1, $"CUST-{Guid.NewGuid():N}".Substring(0, 12), $"user{Guid.NewGuid():N}@example.com", "600123456", statusId));
        customerResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var customer = await customerResponse.Content.ReadFromJsonAsync<CustomerResponse>();
        customer.Should().NotBeNull();

        var taxResponse = await client1.PostAsJsonAsync($"/api/v{taxVersion}/TaxInformation",
            new AddTaxInformationRequest(1, "IVA", "TAX123", customer!.Id));
        taxResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var tax = await taxResponse.Content.ReadFromJsonAsync<TaxInformationResponse>();
        tax.Should().NotBeNull();

        var addressResponse = await client1.PostAsJsonAsync($"/api/v{addressVersion}/Address",
            new AddAddressRequest(1, "Main St", "123", null, null, null, "Center", "Quito", "Pichincha", "EC17001",
                "https://maps.example.com/q", "0", "0", countryId, addressTypeId));
        addressResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var address = await addressResponse.Content.ReadFromJsonAsync<AddressResponse>();
        address.Should().NotBeNull();

        var addResponse = await client1.PostAsJsonAsync($"/api/v{taxAddressVersion}/TaxInformationAddress",
            new AddTaxInformationAddressRequest(1, tax!.Id, address!.Id, true, DateTime.UtcNow.Date, null, true));
        addResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var created = await addResponse.Content.ReadFromJsonAsync<TaxInformationAddressResponse>();
        created.Should().NotBeNull();

        var response = await client2.DeleteAsync($"/api/v{taxAddressVersion}/TaxInformationAddress/{created!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private (HttpClient Tenant1, HttpClient Tenant2) CreateClients()
        => (HttpClientFactory.CreateClientWithTenant(_factory, 1),
            HttpClientFactory.CreateClientWithTenant(_factory, 2));
}
