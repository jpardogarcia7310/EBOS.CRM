using System.Net;
using EBOS.CRM.IntegrationTests.Infrastructure;
using EBOS.CRM.IntegrationTests.TestUtils;
using FluentAssertions;

namespace EBOS.CRM.IntegrationTests.Customer360;

public class Customer360ApiEndpointsSmokeTest(CustomWebApplicationFactory factory)
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _accountContactVersion = ApiVersionHelper.GetLatestVersion(factory, "AccountContact");
    private readonly string _accountContactRoleVersion = ApiVersionHelper.GetLatestVersion(factory, "AccountContactRole");
    private readonly string _accountHierarchyVersion = ApiVersionHelper.GetLatestVersion(factory, "AccountHierarchy");
    private readonly string _customerPreferenceVersion = ApiVersionHelper.GetLatestVersion(factory, "CustomerPreference");
    private readonly string _customerConsentVersion = ApiVersionHelper.GetLatestVersion(factory, "CustomerConsent");
    private readonly string _customerMergeVersion = ApiVersionHelper.GetLatestVersion(factory, "CustomerMerge");

    [Fact]
    public async Task Customer360_GetRoutes_AreExposed()
    {
        await AssertRouteExposedAsync($"/api/v{_accountContactVersion}/AccountContact?tenantId=1");
        await AssertRouteExposedAsync($"/api/v{_accountContactVersion}/AccountContact/1");
        await AssertRouteExposedAsync($"/api/v{_accountContactVersion}/AccountContact/by-account/1?tenantId=1");

        await AssertRouteExposedAsync($"/api/v{_accountContactRoleVersion}/AccountContactRole/1");
        await AssertRouteExposedAsync(
            $"/api/v{_accountContactRoleVersion}/AccountContactRole/by-account-contact/1?tenantId=1");

        await AssertRouteExposedAsync($"/api/v{_accountHierarchyVersion}/AccountHierarchy/1");
        await AssertRouteExposedAsync($"/api/v{_accountHierarchyVersion}/AccountHierarchy/by-account/1?tenantId=1");

        await AssertRouteExposedAsync($"/api/v{_customerPreferenceVersion}/CustomerPreference/by-customer/1?tenantId=1");
        await AssertRouteExposedAsync($"/api/v{_customerConsentVersion}/CustomerConsent/by-customer/1?tenantId=1");
        await AssertRouteExposedAsync(
            $"/api/v{_customerMergeVersion}/CustomerMerge/duplicates?tenantId=1&email=someone@example.com");
        await AssertRouteExposedAsync(
            $"/api/v{_customerMergeVersion}/CustomerMerge/history/by-winner/1?tenantId=1");
        await AssertRouteExposedAsync(
            $"/api/v{_customerMergeVersion}/CustomerMerge/history/by-merged/1?tenantId=1");
    }

    private async Task AssertRouteExposedAsync(string path)
    {
        var response = await _client.GetAsync(path);
        var allowed = new[] { HttpStatusCode.OK, HttpStatusCode.BadRequest, HttpStatusCode.NotFound };
        allowed.Should().Contain(response.StatusCode, $"route must be exposed: {path}");
    }
}
