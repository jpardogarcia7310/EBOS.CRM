using EBOS.CRM.Api.IntegrationTests.Infrastructure;
using EBOS.CRM.Application.Features.Countries.Dtos;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace EBOS.CRM.Api.IntegrationTests.Controllers.Country;

public class PaisesTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task GetAll_Returns_ListOfCountries()
    {
        var response = await _client.GetAsync("/api/v1/Countries");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var countries = await response.Content.ReadFromJsonAsync<List<CountryResponseDto>>();
        countries.Should().NotBeNull();
        countries.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetById_Returns_Country_WhenExists()
    {
        var response = await _client.GetAsync("/api/v1/Countries/1");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var country = await response.Content.ReadFromJsonAsync<CountryResponseDto>();
        country.Should().NotBeNull();
        country!.Name.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetById_Returns_404_WhenNotFound()
    {
        var response = await _client.GetAsync("/api/v1/Countries/999999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}