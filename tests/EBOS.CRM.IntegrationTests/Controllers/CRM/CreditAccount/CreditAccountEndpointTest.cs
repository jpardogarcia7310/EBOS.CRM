using EBOS.CRM.IntegrationTests.Infrastructure;
using EBOS.CRM.IntegrationTests.TestUtils;
using FluentAssertions;

namespace EBOS.CRM.IntegrationTests.Controllers.CRM.CreditAccount;

public class CreditAccountEndpointTest(CustomWebApplicationFactory factory)
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "CreditAccount");

    [Fact]
    public async Task GetAll_ReturnsSuccess()
    {
        var response = await _client.GetAsync($"/api/v{_version}/CreditAccount");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        content.Should().NotBeNullOrWhiteSpace();
    }
}
