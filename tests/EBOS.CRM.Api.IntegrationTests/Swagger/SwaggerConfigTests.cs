using EBOS.CRM.Api.IntegrationTests.Infrastructure;
using FluentAssertions;
using System.Net;

namespace EBOS.CRM.Api.IntegrationTests.Swagger;

public class SwaggerConfigTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task SwaggerUI_Endpoint_IsAvailable()
    {
        var response = await _client.GetAsync("/swagger");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task SwaggerJson_For_V1_IsAvailable()
    {
        var response = await _client.GetAsync("/swagger/v1/swagger.json");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}