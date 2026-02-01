using System.Net;
using EBOS.CRM.Api.IntegrationTests.Infrastructure;
using EBOS.CRM.Api.IntegrationTests.TestUtils;
using FluentAssertions;

namespace EBOS.CRM.Api.IntegrationTests.Controllers.Status;

public class StatusValidationTest(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory);

    [Fact]
    public async Task GetById_Returns_400_WhenIdIsInvalid()
    {
        var response = await _client.GetAsync($"/api/{_version}/Status/-1");
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}





