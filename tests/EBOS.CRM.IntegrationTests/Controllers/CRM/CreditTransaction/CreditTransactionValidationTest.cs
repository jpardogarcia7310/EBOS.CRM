using System.Net;
using EBOS.CRM.IntegrationTests.Infrastructure;
using EBOS.CRM.IntegrationTests.TestUtils;
using FluentAssertions;

namespace EBOS.CRM.IntegrationTests.Controllers.CRM.CreditTransaction;

public class CreditTransactionValidationTest(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "CreditTransaction");

    [Fact]
    public async Task GetById_Returns_400_WhenIdIsInvalid()
    {
        var response = await _client.GetAsync($"/api/v{_version}/CreditTransaction/-1");
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}



