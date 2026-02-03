using EBOS.CRM.Api.IntegrationTests.Infrastructure;
using EBOS.CRM.Api.IntegrationTests.TestUtils;
using FluentAssertions;
using System.Net;

namespace EBOS.CRM.Api.IntegrationTests.ApiVersioning;

public class VersioningTest(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory);

    [Fact]
    public async Task GetAll_LatestVersion_Returns_OK()
    {
        var response = await _client.GetAsync($"/api/v{_version}/Country");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetById_LatestVersion_Returns_OK_WhenExists()
    {
        var response = await _client.GetAsync($"/api/v{_version}/Country/1");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetById_LatestVersion_Returns_404_WhenNotFound()
    {
        var response = await _client.GetAsync($"/api/v{_version}/Country/999999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetAll_LatestVersion_Returns_OK_Repeatable()
    {
        var response = await _client.GetAsync($"/api/v{_version}/Country");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}





