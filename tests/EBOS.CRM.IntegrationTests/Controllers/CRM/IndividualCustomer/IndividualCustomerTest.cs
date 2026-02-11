using System.Net;
using System.Net.Http.Json;
using EBOS.CRM.Application.Contracts.Requests.CRM.IndividualCustomer;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.IntegrationTests.Infrastructure;
using EBOS.CRM.IntegrationTests.TestUtils;
using FluentAssertions;

namespace EBOS.CRM.IntegrationTests.Controllers.CRM.IndividualCustomer;

public class IndividualCustomerTest(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "IndividualCustomer");
    private readonly string _statusVersion = ApiVersionHelper.GetLatestVersion(factory, "Status");
    private readonly string _identificationTypeVersion = ApiVersionHelper.GetLatestVersion(factory, "IdentificationType");

    [Fact]
    public async Task GetAll_Returns_ListOfItems()
    {
        var response = await _client.GetAsync($"/api/v{_version}/IndividualCustomer");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var items = await response.Content.ReadItemsAsync<IndividualCustomerResponse>();
        items.Should().NotBeNull();
    }

    [Fact]
    public async Task GetById_Returns_404_WhenNotFound()
    {
        var response = await _client.GetAsync($"/api/v{_version}/IndividualCustomer/999999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Add_Update_Delete_Works_For_IndividualCustomer()
    {
        var statusId = await LookupHelper.GetStatusIdAsync(_client, _statusVersion);
        var identificationTypeId =
            await LookupHelper.GetIdentificationTypeIdAsync(_client, _identificationTypeVersion);

        var addRequest = new AddIndividualCustomerRequest(
            TenantId: 1,
            Code: $"IND-{Guid.NewGuid():N}".Substring(0, 12),
            Email: $"ind{Guid.NewGuid():N}@example.com",
            Phone: "+34 600 555 000",
            StatusId: statusId,
            FirstName: "Jane",
            LastName: "Doe",
            BirthDate: new DateTime(1990, 5, 20),
            IdentificationNumber: "1234567890",
            IdentificationTypeId: identificationTypeId);

        var addResponse = await _client.PostAsJsonAsync($"/api/v{_version}/IndividualCustomer", addRequest);
        addResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var created = await addResponse.Content.ReadFromJsonAsync<IndividualCustomerResponse>();
        created.Should().NotBeNull();

        var updateRequest = new UpdateIndividualCustomerRequest(
            TenantId: 1,
            Code: $"IND-{Guid.NewGuid():N}".Substring(0, 12),
            Email: $"ind{Guid.NewGuid():N}@example.com",
            Phone: "+34 600 777 000",
            StatusId: statusId,
            FirstName: "Jane",
            LastName: "Doe",
            BirthDate: new DateTime(1990, 5, 20),
            IdentificationNumber: "9876543210",
            IdentificationTypeId: identificationTypeId);

        var updateResponse =
            await _client.PutAsJsonAsync($"/api/v{_version}/IndividualCustomer/{created!.Id}", updateRequest);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var deleteResponse = await _client.DeleteAsync($"/api/v{_version}/IndividualCustomer/{created.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}





