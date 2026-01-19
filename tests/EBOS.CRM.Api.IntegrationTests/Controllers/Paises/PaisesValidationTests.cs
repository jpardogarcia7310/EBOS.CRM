using EBOS.CRM.Api.IntegrationTests.Infrastructure;
using FluentAssertions;
using System.Net;

namespace EBOS.CRM.Api.IntegrationTests.Controllers.Country;

public class PaisesValidationTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task GetById_NotFound_Returns_404()
    {
        var response = await _client.GetAsync("/api/v1/Countries/999999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}