using System.Net;
using System.Net.Http.Json;
using EBOS.CRM.Api.IntegrationTests.Infrastructure;
using EBOS.CRM.Application.Contracts.Responses;
using FluentAssertions;

namespace EBOS.CRM.Api.IntegrationTests.Controllers.Status;

public class StatusTest(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task GetAll_Returns_ListOfCountries()
    {
        var response = await _client.GetAsync("/api/v1/Status");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var statuses = await response.Content.ReadFromJsonAsync<List<StatusResponse>>();
        statuses.Should().NotBeNull();
        statuses.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetById_Returns_Country_WhenExists()
    {
        var response = await _client.GetAsync("/api/v1/Status/1");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var status = await response.Content.ReadFromJsonAsync<StatusResponse>();
        status.Should().NotBeNull();
        status.Description.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetById_Returns_404_WhenNotFound()
    {
        var response = await _client.GetAsync("/api/v1/Status/999999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
