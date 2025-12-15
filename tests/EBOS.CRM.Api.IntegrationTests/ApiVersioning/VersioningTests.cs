using EBOS.CRM.Api.IntegrationTests.Infrastructure;
using FluentAssertions;
using System.Net;

namespace EBOS.CRM.Api.IntegrationTests.ApiVersioning;

public class VersioningTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task GetAll_V1_Returns_OK()
    {
        var response = await _client.GetAsync("/api/v1/Countries");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetById_V1_Returns_OK_WhenExists()
    {
        var response = await _client.GetAsync("/api/v1/Countries/1");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetById_V1_Returns_404_WhenNotFound()
    {
        var response = await _client.GetAsync("/api/v1/Countries/999999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // Si en el futuro añades v2, puedes habilitar estos tests:
    //[Fact]
    //public async Task GetAll_V2_Returns_OK()
    //{
    //    var response = await _client.GetAsync("/api/v2/Countries");
    //    response.StatusCode.Should().Be(HttpStatusCode.OK);
    //}
}