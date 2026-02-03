using System.Net;
using System.Net.Http.Json;
using EBOS.CRM.Api.IntegrationTests.Infrastructure;
using EBOS.CRM.Api.IntegrationTests.TestUtils;
using EBOS.CRM.Application.Contracts.Responses;
using FluentAssertions;

namespace EBOS.CRM.Api.IntegrationTests.Controllers.Status;

public class StatusTest(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory);

    [Fact]
    public async Task GetAll_Returns_ListOfItems()
    {
        var response = await _client.GetAsync($"/api/v{_version}/Status");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var statuses = await response.Content.ReadPagedItemsAsync<StatusResponse>();
        statuses.Should().NotBeNull();
        statuses.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetById_Returns_Status_WhenExists()
    {
        var response = await _client.GetAsync($"/api/v{_version}/Status/1");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var status = await response.Content.ReadFromJsonAsync<StatusResponse>();
        status.Should().NotBeNull();
        status.Description.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetById_Returns_404_WhenNotFound()
    {
        var response = await _client.GetAsync($"/api/v{_version}/Status/999999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}







