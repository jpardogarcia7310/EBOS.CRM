using System.Net.Http.Json;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.IntegrationTests.Infrastructure;
using EBOS.CRM.IntegrationTests.TestUtils;
using FluentAssertions;

namespace EBOS.CRM.IntegrationTests.Controllers.CRM.AccountContactRole;

public class AccountContactRoleEndpointTest(CustomWebApplicationFactory factory)
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "AccountContactRole");
    private readonly string _contactVersion = ApiVersionHelper.GetLatestVersion(factory, "AccountContact");

    [Fact]
    public async Task GetByAccountContact_ReturnsSuccess()
    {
        var contactId = await ControllerTestHelperGetFirstIdAsync<AccountContactResponse>(
            _client, $"/api/v{_contactVersion}/AccountContact?tenantId=1", x => x.Id);
        if (contactId == 0) return;

        var response = await _client.GetAsync(
            $"/api/v{_version}/AccountContactRole/by-account-contact/{contactId}?tenantId=1");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        content.Should().NotBeNull();
    }

    private static async Task<long> ControllerTestHelperGetFirstIdAsync<T>(
        HttpClient client, string url, Func<T, long> selector)
    {
        var response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();
        var items = await response.Content.ReadFromJsonAsync<List<T>>();
        items.Should().NotBeNull();
        if (items!.Count == 0)
        {
            return 0;
        }
        return selector(items[0]);
    }
}
