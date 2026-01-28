using System.Net;
using System.Net.Http.Json;
using EBOS.CRM.Api.IntegrationTests.Infrastructure;
using EBOS.CRM.Application.Contracts.Responses;
using FluentAssertions;

namespace EBOS.CRM.Api.IntegrationTests.Controllers.Country;

public class CountryTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task GetAll_Returns_ListOfCountries()
    {
        var response = await _client.GetAsync("/api/v1/Country");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var countries = await response.Content.ReadFromJsonAsync<List<CountryResponse>>();
        countries.Should().NotBeNull();
        countries.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetById_Returns_Country_WhenExists()
    {
        var response = await _client.GetAsync("/api/v1/Country/1");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var country = await response.Content.ReadFromJsonAsync<CountryResponse>();
        country.Should().NotBeNull();
        country.Name.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetById_Returns_404_WhenNotFound()
    {
        var response = await _client.GetAsync("/api/v1/Country/999999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
