using System.Net;
using System.Net.Http.Json;
using EBOS.CRM.Contracts.Requests.CRM.Customer;
using EBOS.CRM.Contracts.Requests.CRM.TaxInformation;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.IntegrationTests.Infrastructure;
using EBOS.CRM.IntegrationTests.TestUtils;
using FluentAssertions;

namespace EBOS.CRM.IntegrationTests.Controllers.CRM.TaxInformation;

public class TaxInformationTest(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "TaxInformation");
    private readonly string _customerVersion = ApiVersionHelper.GetLatestVersion(factory, "Customer");
    private readonly string _statusVersion = ApiVersionHelper.GetLatestVersion(factory, "Status");

    [Fact]
    public async Task GetAll_Returns_ListOfItems()
    {
        var response = await _client.GetAsync($"/api/v{_version}/TaxInformation");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var items = await response.Content.ReadItemsAsync<TaxInformationResponse>();
        items.Should().NotBeNull();
    }

    [Fact]
    public async Task GetById_Returns_404_WhenNotFound()
    {
        var response = await _client.GetAsync($"/api/v{_version}/TaxInformation/999999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Add_Update_Delete_Works_For_TaxInformation()
    {
        var customerId = await CreateCustomerAsync();

        var addRequest = new AddTaxInformationRequest(
            TenantId: 1,
            TaxName: "IVA",
            TaxIdentificationNumber: "ESB12345678",
            CustomerId: customerId);

        var addResponse = await _client.PostAsJsonAsync($"/api/v{_version}/TaxInformation", addRequest);
        addResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var created = await addResponse.Content.ReadFromJsonAsync<TaxInformationResponse>();
        created.Should().NotBeNull();

        var updateRequest = new UpdateTaxInformationRequest(
            Id: created.Id,
            TenantId: 1,
            TaxName: "IVA Updated",
            TaxIdentificationNumber: "ESB87654321",
            CustomerId: customerId);

        var updateResponse =
            await _client.PutAsJsonAsync($"/api/v{_version}/TaxInformation/{created.Id}", updateRequest);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var deleteResponse = await _client.DeleteAsync($"/api/v{_version}/TaxInformation/{created.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    private async Task<long> CreateCustomerAsync()
    {
        var statusId = await LookupHelper.GetStatusIdAsync(_client, _statusVersion);

        var request = new AddCustomerRequest(
            TenantId: 1,
            Code: $"CUST-{Guid.NewGuid():N}".Substring(0, 12),
            Email: $"user{Guid.NewGuid():N}@example.com",
            Phone: "+34 600 123 456",
            StatusId: statusId);

        var response = await _client.PostAsJsonAsync($"/api/v{_customerVersion}/Customer", request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var created = await response.Content.ReadFromJsonAsync<CustomerResponse>();
        created.Should().NotBeNull();
        return created.Id;
    }
}





