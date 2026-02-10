using System.Net;
using EBOS.CRM.Api.IntegrationTests.Infrastructure;
using EBOS.CRM.Api.IntegrationTests.TestUtils;
using EBOS.CRM.Application.Contracts.Requests.CRM.CorporateCustomer;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using FluentAssertions;
using System.Net.Http.Json;

namespace EBOS.CRM.Api.IntegrationTests.Controllers.CRM.CorporateCustomer;

public class CorporateCustomerTest(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "CorporateCustomer");
    private readonly string _statusVersion = ApiVersionHelper.GetLatestVersion(factory, "Status");

    [Fact]
    public async Task GetAll_Returns_ListOfItems()
    {
        var response = await _client.GetAsync($"/api/v{_version}/CorporateCustomer");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var items = await response.Content.ReadItemsAsync<CorporateCustomerResponse>();
        items.Should().NotBeNull();
    }

    [Fact]
    public async Task GetById_Returns_404_WhenNotFound()
    {
        var response = await _client.GetAsync($"/api/v{_version}/CorporateCustomer/999999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Add_Update_Delete_Works_For_CorporateCustomer()
    {
        var statusId = await LookupHelper.GetStatusIdAsync(_client, _statusVersion);

        var addRequest = new AddCorporateCustomerRequest(
            TenantId: 1,
            Code: $"CORP-{Guid.NewGuid():N}".Substring(0, 12),
            Email: $"corp{Guid.NewGuid():N}@example.com",
            Phone: "+34 911 000 111",
            StatusId: statusId,
            LegalName: "Contoso S.A.",
            TaxIdentification: "B12345678");

        var addResponse = await _client.PostAsJsonAsync($"/api/v{_version}/CorporateCustomer", addRequest);
        addResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var created = await addResponse.Content.ReadFromJsonAsync<CorporateCustomerResponse>();
        created.Should().NotBeNull();

        var updateRequest = new UpdateCorporateCustomerRequest(
            TenantId: 1,
            Code: $"CORP-{Guid.NewGuid():N}".Substring(0, 12),
            Email: $"corp{Guid.NewGuid():N}@example.com",
            Phone: "+34 911 000 999",
            StatusId: statusId,
            LegalName: "Contoso Updated",
            TaxIdentification: "B99999999");

        var updateResponse =
            await _client.PutAsJsonAsync($"/api/v{_version}/CorporateCustomer/{created!.Id}", updateRequest);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var deleteResponse = await _client.DeleteAsync($"/api/v{_version}/CorporateCustomer/{created.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}





