using System.Net;
using System.Net.Http.Json;
using EBOS.CRM.Api.IntegrationTests.Infrastructure;
using EBOS.CRM.Application.Contracts.Responses;
using FluentAssertions;

namespace EBOS.CRM.Api.IntegrationTests.Controllers.AddressType;

public class AddressTypeTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task GetAll_Returns_ListOfAddressTypes()
    {
        var response = await _client.GetAsync("/api/v2/AddressType");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var items = await response.Content.ReadFromJsonAsync<List<AddressTypeResponse>>();
        items.Should().NotBeNull();
        items.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetById_Returns_AddressType_WhenExists()
    {
        var response = await _client.GetAsync("/api/v2/AddressType/1");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var item = await response.Content.ReadFromJsonAsync<AddressTypeResponse>();
        item.Should().NotBeNull();
        item!.Description.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetById_Returns_404_WhenNotFound()
    {
        var response = await _client.GetAsync("/api/v2/AddressType/999999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
