using System.Net;
using System.Net.Http.Json;
using EBOS.CRM.Application.Contracts.Requests.CRM.Lead;
using EBOS.CRM.IntegrationTests.Infrastructure;
using EBOS.CRM.IntegrationTests.TestUtils;
using FluentAssertions;

namespace EBOS.CRM.IntegrationTests.Controllers.CRM.Lead;

public class LeadValidationTest(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "Lead");

    [Fact]
    public async Task GetById_Returns_404_WhenIdIsInvalid()
    {
        var response = await _client.GetAsync($"/api/v{_version}/Lead/-1");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Add_Returns_400_WhenRequestIsInvalid()
    {
        var request = new AddLeadRequest(
            TenantId: 1,
            Source: "",
            Status: "",
            OwnerUserId: 0,
            CompanyName: "",
            ContactName: "",
            Email: "",
            Phone: "",
            EstimatedValue: -10m,
            Notes: new string('x', 2501));

        var response = await _client.PostAsJsonAsync($"/api/v{_version}/Lead", request);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Update_Returns_400_WhenRequestIsInvalid()
    {
        var request = new UpdateLeadRequest(
            Id: 0,
            TenantId: 1,
            Source: "",
            Status: "",
            OwnerUserId: 0,
            CompanyName: "",
            ContactName: "",
            Email: "",
            Phone: "",
            EstimatedValue: -5m,
            Notes: new string('x', 2501));

        var response = await _client.PutAsJsonAsync($"/api/v{_version}/Lead/1", request);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
