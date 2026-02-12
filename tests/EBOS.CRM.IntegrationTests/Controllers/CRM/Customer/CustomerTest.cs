using System.Net;
using System.Net.Http.Json;
using EBOS.CRM.Contracts.Requests.CRM.Customer;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.IntegrationTests.Infrastructure;
using EBOS.CRM.IntegrationTests.TestUtils;
using FluentAssertions;

namespace EBOS.CRM.IntegrationTests.Controllers.CRM.Customer;

public class CustomerTest(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "Customer");
    private readonly string _statusVersion = ApiVersionHelper.GetLatestVersion(factory, "Status");

    [Fact]
    public async Task GetAll_Returns_ListOfItems()
    {
        var response = await _client.GetAsync($"/api/v{_version}/Customer");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var items = await response.Content.ReadItemsAsync<CustomerResponse>();
        items.Should().NotBeNull();
    }

    [Fact]
    public async Task GetById_Returns_404_WhenNotFound()
    {
        var response = await _client.GetAsync($"/api/v{_version}/Customer/999999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Add_Update_Delete_Works_For_Customer()
    {
        var statusId = await LookupHelper.GetStatusIdAsync(_client, _statusVersion);

        var addRequest = new AddCustomerRequest(
            TenantId: 1,
            Code: $"CUST-{Guid.NewGuid():N}".Substring(0, 12),
            Email: $"user{Guid.NewGuid():N}@example.com",
            Phone: "+34 600 123 456",
            StatusId: statusId);

        var addResponse = await _client.PostAsJsonAsync($"/api/v{_version}/Customer", addRequest);
        addResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var created = await addResponse.Content.ReadFromJsonAsync<CustomerResponse>();
        created.Should().NotBeNull();

        var updateRequest = new UpdateCustomerRequest(
            Id: created.Id,
            TenantId: 1,
            Code: $"CUST-{Guid.NewGuid():N}".Substring(0, 12),
            Email: $"user{Guid.NewGuid():N}@example.com",
            Phone: "+34 600 999 000",
            StatusId: statusId);

        var updateResponse =
            await _client.PutAsJsonAsync($"/api/v{_version}/Customer/{created.Id}", updateRequest);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var deleteResponse = await _client.DeleteAsync($"/api/v{_version}/Customer/{created.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}





