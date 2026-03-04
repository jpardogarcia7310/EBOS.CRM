using System.Net;
using System.Net.Http.Json;
using EBOS.CRM.Contracts.Requests.CRM.Address;
using EBOS.CRM.Contracts.Requests.CRM.BankInformation;
using EBOS.CRM.Contracts.Requests.CRM.BranchOffice;
using EBOS.CRM.Contracts.Requests.CRM.BranchOfficeAddress;
using EBOS.CRM.Contracts.Requests.CRM.CorporateCustomer;
using EBOS.CRM.Contracts.Requests.CRM.CreditAccount;
using EBOS.CRM.Contracts.Requests.CRM.CreditTransaction;
using EBOS.CRM.Contracts.Requests.CRM.Customer;
using EBOS.CRM.Contracts.Requests.CRM.CustomerAddress;
using EBOS.CRM.Contracts.Requests.CRM.IndividualCustomer;
using EBOS.CRM.Contracts.Requests.CRM.TaxInformation;
using EBOS.CRM.Contracts.Requests.CRM.TaxInformationAddress;
using EBOS.CRM.IntegrationTests.Infrastructure;
using EBOS.CRM.IntegrationTests.TestUtils;
using FluentAssertions;

namespace EBOS.CRM.IntegrationTests.Controllers.CRM;

public class EndpointValidationNegativeTest(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task AddCustomer_Returns_400_When_Required_Fields_Missing()
    {
        var version = ApiVersionHelper.GetLatestVersion(factory, "Customer");
        var request = new AddCustomerRequest(1, "", "", "", 0);

        var response = await _client.PostAsJsonAsync($"/api/v{version}/Customer", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AddAddress_Returns_400_When_Required_Fields_Missing()
    {
        var version = ApiVersionHelper.GetLatestVersion(factory, "Address");
        var request = new AddAddressRequest(1, "", "", null, null, null, null, "", "", "", null, null, null, 0, 0);

        var response = await _client.PostAsJsonAsync($"/api/v{version}/Address", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AddCorporateCustomer_Returns_400_When_Required_Fields_Missing()
    {
        var version = ApiVersionHelper.GetLatestVersion(factory, "CorporateCustomer");
        var request = new AddCorporateCustomerRequest(1, "", "", "", 0, "", "");

        var response = await _client.PostAsJsonAsync($"/api/v{version}/CorporateCustomer", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AddIndividualCustomer_Returns_400_When_Required_Fields_Missing()
    {
        var version = ApiVersionHelper.GetLatestVersion(factory, "IndividualCustomer");
        var request = new AddIndividualCustomerRequest(1, "", "", "", 0, "", "", DateTime.UtcNow, null, 0);

        var response = await _client.PostAsJsonAsync($"/api/v{version}/IndividualCustomer", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AddBranchOffice_Returns_400_When_Required_Fields_Missing()
    {
        var version = ApiVersionHelper.GetLatestVersion(factory, "BranchOffice");
        var request = new AddBranchOfficeRequest(1, "", "", 0);

        var response = await _client.PostAsJsonAsync($"/api/v{version}/BranchOffice", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AddBranchOfficeAddress_Returns_400_When_Required_Fields_Invalid()
    {
        var version = ApiVersionHelper.GetLatestVersion(factory, "BranchOfficeAddress");
        var request = new AddBranchOfficeAddressRequest(1, 0, 0, true, DateTime.UtcNow.Date, null, true);

        var response = await _client.PostAsJsonAsync($"/api/v{version}/BranchOfficeAddress", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AddBankInformation_Returns_400_When_Required_Fields_Missing()
    {
        var version = ApiVersionHelper.GetLatestVersion(factory, "BankInformation");
        var request = new AddBankInformationRequest(1, "", null, null, 0);

        var response = await _client.PostAsJsonAsync($"/api/v{version}/BankInformation", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AddCreditAccount_Returns_400_When_Required_Fields_Invalid()
    {
        var version = ApiVersionHelper.GetLatestVersion(factory, "CreditAccount");
        var request = new AddCreditAccountRequest(1, 0, 0, 0);

        var response = await _client.PostAsJsonAsync($"/api/v{version}/CreditAccount", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AddCreditTransaction_Returns_400_When_Required_Fields_Missing()
    {
        var version = ApiVersionHelper.GetLatestVersion(factory, "CreditTransaction");
        var request = new AddCreditTransactionRequest(1, DateTime.UtcNow, 0, "", "", "", 0);

        var response = await _client.PostAsJsonAsync($"/api/v{version}/CreditTransaction", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AddCustomerAddress_Returns_400_When_Required_Fields_Invalid()
    {
        var version = ApiVersionHelper.GetLatestVersion(factory, "CustomerAddress");
        var request = new AddCustomerAddressRequest(1, 0, 0, true, DateTime.UtcNow.Date, null, true);

        var response = await _client.PostAsJsonAsync($"/api/v{version}/CustomerAddress", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AddTaxInformation_Returns_400_When_Required_Fields_Missing()
    {
        var version = ApiVersionHelper.GetLatestVersion(factory, "TaxInformation");
        var request = new AddTaxInformationRequest(1, "", "", 0);

        var response = await _client.PostAsJsonAsync($"/api/v{version}/TaxInformation", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AddTaxInformationAddress_Returns_400_When_Required_Fields_Invalid()
    {
        var version = ApiVersionHelper.GetLatestVersion(factory, "TaxInformationAddress");
        var request = new AddTaxInformationAddressRequest(1, 0, 0, true, DateTime.UtcNow.Date, null, true);

        var response = await _client.PostAsJsonAsync($"/api/v{version}/TaxInformationAddress", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
