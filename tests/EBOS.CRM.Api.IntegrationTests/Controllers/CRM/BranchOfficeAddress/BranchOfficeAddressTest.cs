using System.Net;
using System.Net.Http.Json;
using EBOS.CRM.Api.IntegrationTests.Infrastructure;
using EBOS.CRM.Api.IntegrationTests.TestUtils;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using FluentAssertions;

namespace EBOS.CRM.Api.IntegrationTests.Controllers.CRM.BranchOfficeAddress;

public class BranchOfficeAddressTest(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory);

    [Fact]
    public async Task GetAll_Returns_ListOfItems()
    {
        var response = await _client.GetAsync($"/api/{_version}/BranchOfficeAddress");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var items = await response.Content.ReadPagedItemsAsync<BranchOfficeAddressResponse>();
        items.Should().NotBeNull();
    }

    [Fact]
    public async Task GetById_Returns_404_WhenNotFound()
    {
        var response = await _client.GetAsync($"/api/{_version}/BranchOfficeAddress/999999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}

