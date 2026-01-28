using System.Net;
using EBOS.CRM.Api.IntegrationTests.Infrastructure;
using FluentAssertions;

namespace EBOS.CRM.Api.IntegrationTests.Controllers.Country;

public class CountriesValidationTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task GetById_NotFound_Returns_404()
    {
        var response = await _client.GetAsync("/api/v1/Country/999999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
