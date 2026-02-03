using EBOS.CRM.Api.IntegrationTests.Infrastructure;
using FluentAssertions;
using System.Net;
using EBOS.CRM.Api.IntegrationTests.TestUtils;

namespace EBOS.CRM.Api.IntegrationTests.Swagger;

public class SwaggerConfigTest(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory);

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



