using System.Net;
using System.Net.Http.Json;
using EBOS.CRM.Api.IntegrationTests.Infrastructure;
using EBOS.CRM.Api.IntegrationTests.TestUtils;
using EBOS.CRM.Application.Contracts.Responses;
using FluentAssertions;

namespace EBOS.CRM.Api.IntegrationTests.Controllers.Country;

public class CountryTest(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory);

    [Fact]
    public async Task GetAll_Returns_ListOfItems()
    {
        var response = await _client.GetAsync($"/api/v{_version}/Country");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var countries = await response.Content.ReadPagedItemsAsync<CountryResponse>();
        countries.Should().NotBeNull();
        countries.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetById_Returns_Country_WhenExists()
    {
        var response = await _client.GetAsync($"/api/v{_version}/Country/1");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var country = await response.Content.ReadFromJsonAsync<CountryResponse>();
        country.Should().NotBeNull();
        country.Name.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetById_Returns_404_WhenNotFound()
    {
        var response = await _client.GetAsync($"/api/v{_version}/Country/999999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}

