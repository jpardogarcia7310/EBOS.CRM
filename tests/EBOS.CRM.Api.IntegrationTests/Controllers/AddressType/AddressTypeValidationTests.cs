using System.Net;
using EBOS.CRM.Api.IntegrationTests.Infrastructure;
using FluentAssertions;

namespace EBOS.CRM.Api.IntegrationTests.Controllers.AddressType;

public class AddressTypeValidationTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task GetById_Returns_400_WhenIdIsInvalid()
    {
        var response = await _client.GetAsync("/api/v2/AddressType/-1");
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
