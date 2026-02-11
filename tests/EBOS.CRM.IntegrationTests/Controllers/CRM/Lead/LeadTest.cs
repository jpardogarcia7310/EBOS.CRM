using System.Net;
using System.Net.Http.Json;
using EBOS.CRM.Application.Contracts.Requests.CRM.Lead;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.IntegrationTests.Infrastructure;
using EBOS.CRM.IntegrationTests.TestUtils;
using FluentAssertions;

namespace EBOS.CRM.IntegrationTests.Controllers.CRM.Lead;

public class LeadTest(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "Lead");

    [Fact]
    public async Task GetAll_Returns_ListOfItems()
    {
        var response = await _client.GetAsync($"/api/v{_version}/Lead");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var items = await response.Content.ReadItemsAsync<LeadResponse>();
        items.Should().NotBeNull();
    }

    [Fact]
    public async Task GetById_Returns_404_WhenNotFound()
    {
        var response = await _client.GetAsync($"/api/v{_version}/Lead/999999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Add_Update_Works_For_Lead()
    {
        var addRequest = new AddLeadRequest(
            TenantId: 1,
            Source: "Web",
            Status: "New",
            OwnerUserId: 1,
            CompanyName: $"Company-{Guid.NewGuid():N}",
            ContactName: "Jane Doe",
            Email: $"lead{Guid.NewGuid():N}@example.com",
            Phone: "1234567890",
            EstimatedValue: 2500m,
            Notes: "Initial lead");

        var addResponse = await _client.PostAsJsonAsync($"/api/v{_version}/Lead", addRequest);
        addResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var created = await addResponse.Content.ReadFromJsonAsync<LeadResponse>();
        created.Should().NotBeNull();

        var updateRequest = new UpdateLeadRequest(
            Id: created!.Id,
            TenantId: 1,
            Source: "Referral",
            Status: "Contacted",
            OwnerUserId: 1,
            CompanyName: created.CompanyName,
            ContactName: "Jane Doe",
            Email: created.Email,
            Phone: "1234567890",
            EstimatedValue: 5000m,
            Notes: "Updated lead");

        var updateResponse = await _client.PutAsJsonAsync($"/api/v{_version}/Lead/{created.Id}", updateRequest);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var updated = await updateResponse.Content.ReadFromJsonAsync<LeadResponse>();
        updated.Should().NotBeNull();
        updated!.Status.Should().Be("Contacted");
    }
}
