using System.Net;
using EBOS.CRM.IntegrationTests.Infrastructure;
using EBOS.CRM.IntegrationTests.TestUtils;
using FluentAssertions;

namespace EBOS.CRM.IntegrationTests.Customer360;

public class Customer360AuthorizationMatrixTest(CustomWebApplicationFactory factory)
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _authorized = factory.CreateClient();
    private readonly HttpClient _unauthorized = factory.CreateClient();
    private readonly string _customerConsentVersion = ApiVersionHelper.GetLatestVersion(factory, "CustomerConsent");
    private readonly string _customerMergeVersion = ApiVersionHelper.GetLatestVersion(factory, "CustomerMerge");
    private readonly string _customerPreferenceVersion = ApiVersionHelper.GetLatestVersion(factory, "CustomerPreference");
    private readonly string _customerPrivacyVersion = ApiVersionHelper.GetLatestVersion(factory, "CustomerPrivacy");

    [Fact]
    public async Task Customer360_SensitiveGetEndpoints_RequireAuthentication()
    {
        _authorized.DefaultRequestHeaders.Remove("X-Test-Auth");
        _unauthorized.DefaultRequestHeaders.Remove("X-Test-Auth");
        _unauthorized.DefaultRequestHeaders.Add(TestAuthHandler.AuthModeHeader, "none");

        var routes = new[]
        {
            $"/api/v{_customerConsentVersion}/CustomerConsent/by-customer/1?tenantId=1",
            $"/api/v{_customerPreferenceVersion}/CustomerPreference/by-customer/1?tenantId=1",
            $"/api/v{_customerMergeVersion}/CustomerMerge/duplicates?tenantId=1&email=x@example.com",
            $"/api/v{_customerMergeVersion}/CustomerMerge/history/by-winner/1?tenantId=1",
            $"/api/v{_customerMergeVersion}/CustomerMerge/history/by-merged/1?tenantId=1",
            $"/api/v{_customerPrivacyVersion}/CustomerPrivacy/by-customer/1?tenantId=1",
            $"/api/v{_customerPrivacyVersion}/CustomerPrivacy/1?tenantId=1",
            $"/api/v{_customerPrivacyVersion}/CustomerPrivacy/by-status/PENDING?tenantId=1"
        };

        foreach (var route in routes)
        {
            var denied = await _unauthorized.GetAsync(route);
            denied.StatusCode.Should().Be(HttpStatusCode.Unauthorized, $"endpoint must require auth: {route}");

            var allowed = await _authorized.GetAsync(route);
            var expected = new[] { HttpStatusCode.OK, HttpStatusCode.BadRequest, HttpStatusCode.NotFound };
            expected.Should().Contain(allowed.StatusCode, $"endpoint must be reachable with auth: {route}");
        }
    }
}
