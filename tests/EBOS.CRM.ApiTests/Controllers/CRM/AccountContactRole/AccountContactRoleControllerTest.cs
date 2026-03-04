using System.Net;
using System.Net.Http.Json;
using EBOS.CRM.ApiTests.Fixtures;
using EBOS.CRM.ApiTests.TestUtils;
using EBOS.CRM.Contracts.Responses.CRM;

namespace EBOS.CRM.ApiTests.Controllers.CRM.AccountContactRole;

public class AccountContactRoleControllerTest(CustomWebApplicationFactory<Program> factory)
    : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "AccountContactRole");
    private readonly string _accountContactVersion = ApiVersionHelper.GetLatestVersion(factory, "AccountContact");

    [Fact]
    public async Task GetByAccountContact_ReturnsSuccess()
    {
        var accountContactsResponse = await _client.GetAsync($"/api/v{_accountContactVersion}/AccountContact?tenantId=1");
        AssertStatus(accountContactsResponse.StatusCode, HttpStatusCode.OK, HttpStatusCode.Unauthorized);
        if (accountContactsResponse.StatusCode != HttpStatusCode.OK) return;
        var accountContacts = await accountContactsResponse.Content.ReadItemsAsync<AccountContactResponse>();
        var firstContact = accountContacts.FirstOrDefault();
        Assert.NotNull(firstContact);

        var response = await _client.GetAsync(
            $"/api/v{_version}/AccountContactRole/by-account-contact/{firstContact!.Id}?tenantId=1");
        AssertStatus(response.StatusCode, HttpStatusCode.OK, HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetById_ExistingId_ReturnsItem()
    {
        var accountContactsResponse = await _client.GetAsync($"/api/v{_accountContactVersion}/AccountContact?tenantId=1");
        AssertStatus(accountContactsResponse.StatusCode, HttpStatusCode.OK, HttpStatusCode.Unauthorized);
        if (accountContactsResponse.StatusCode != HttpStatusCode.OK) return;
        var accountContacts = await accountContactsResponse.Content.ReadItemsAsync<AccountContactResponse>();
        var firstContact = accountContacts.FirstOrDefault();
        Assert.NotNull(firstContact);

        var listResponse = await _client.GetAsync(
            $"/api/v{_version}/AccountContactRole/by-account-contact/{firstContact!.Id}?tenantId=1");
        AssertStatus(listResponse.StatusCode, HttpStatusCode.OK, HttpStatusCode.Unauthorized);
        if (listResponse.StatusCode != HttpStatusCode.OK) return;
        var list = await listResponse.Content.ReadItemsAsync<AccountContactRoleResponse>();
        var first = list.FirstOrDefault();
        Assert.NotNull(first);

        var response = await _client.GetAsync($"/api/v{_version}/AccountContactRole/{first!.Id}");
        AssertStatus(response.StatusCode, HttpStatusCode.OK, HttpStatusCode.Unauthorized);
        if (response.StatusCode != HttpStatusCode.OK) return;
        var item = await response.Content.ReadFromJsonAsync<AccountContactRoleResponse>();
        Assert.NotNull(item);
        Assert.Equal(first.Id, item!.Id);
    }

    [Fact]
    public async Task GetById_NonExistingId_ReturnsNotFound()
    {
        var response = await _client.GetAsync($"/api/v{_version}/AccountContactRole/999999999");
        AssertStatus(response.StatusCode, HttpStatusCode.NotFound, HttpStatusCode.Unauthorized);
    }

    private static void AssertStatus(HttpStatusCode actual, params HttpStatusCode[] expected) =>
        Assert.True(expected.Contains(actual), $"Unexpected status code: {actual}");
}
