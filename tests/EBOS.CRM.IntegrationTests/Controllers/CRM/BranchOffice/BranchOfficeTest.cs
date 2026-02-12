using System.Net;
using System.Net.Http.Json;
using EBOS.CRM.Contracts.Requests.CRM.BranchOffice;
using EBOS.CRM.Contracts.Requests.CRM.CorporateCustomer;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.IntegrationTests.Infrastructure;
using EBOS.CRM.IntegrationTests.TestUtils;
using FluentAssertions;

namespace EBOS.CRM.IntegrationTests.Controllers.CRM.BranchOffice;

public class BranchOfficeTest(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "BranchOffice");
    private readonly string _corporateVersion = ApiVersionHelper.GetLatestVersion(factory, "CorporateCustomer");
    private readonly string _statusVersion = ApiVersionHelper.GetLatestVersion(factory, "Status");

    [Fact]
    public async Task GetAll_Returns_ListOfItems()
    {
        var response = await _client.GetAsync($"/api/v{_version}/BranchOffice");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var items = await response.Content.ReadItemsAsync<BranchOfficeResponse>();
        items.Should().NotBeNull();
    }

    [Fact]
    public async Task GetById_Returns_404_WhenNotFound()
    {
        var response = await _client.GetAsync($"/api/v{_version}/BranchOffice/999999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Add_Update_Delete_Works_For_BranchOffice()
    {
        var corporateCustomerId = await CreateCorporateCustomerAsync();

        var addRequest = new AddBranchOfficeRequest(
            TenantId: 1,
            Name: "HQ Madrid",
            PhoneNumber: "+34 911 000 222",
            CorporateCustomerId: corporateCustomerId);

        var addResponse = await _client.PostAsJsonAsync($"/api/v{_version}/BranchOffice", addRequest);
        addResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var created = await addResponse.Content.ReadFromJsonAsync<BranchOfficeResponse>();
        created.Should().NotBeNull();

        var updateRequest = new UpdateBranchOfficeRequest(
            Id: created.Id,
            TenantId: 1,
            Name: "HQ Updated",
            PhoneNumber: "+34 911 999 000",
            CorporateCustomerId: corporateCustomerId);

        var updateResponse =
            await _client.PutAsJsonAsync($"/api/v{_version}/BranchOffice/{created.Id}", updateRequest);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var deleteResponse = await _client.DeleteAsync($"/api/v{_version}/BranchOffice/{created.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    private async Task<long> CreateCorporateCustomerAsync()
    {
        var statusId = await LookupHelper.GetStatusIdAsync(_client, _statusVersion);

        var request = new AddCorporateCustomerRequest(
            TenantId: 1,
            Code: $"CORP-{Guid.NewGuid():N}".Substring(0, 12),
            Email: $"corp{Guid.NewGuid():N}@example.com",
            Phone: "+34 911 000 111",
            StatusId: statusId,
            LegalName: "Contoso S.A.",
            TaxIdentification: "B12345678");

        var response = await _client.PostAsJsonAsync($"/api/v{_corporateVersion}/CorporateCustomer", request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var created = await response.Content.ReadFromJsonAsync<CorporateCustomerResponse>();
        created.Should().NotBeNull();
        return created.Id;
    }
}





