using System.Net;
using EBOS.CRM.IntegrationTests.Infrastructure;
using EBOS.CRM.IntegrationTests.TestUtils;
using FluentAssertions;

namespace EBOS.CRM.IntegrationTests.Swagger;

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



