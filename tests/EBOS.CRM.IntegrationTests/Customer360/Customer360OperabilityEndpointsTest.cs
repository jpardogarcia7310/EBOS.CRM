using System.Net;
using System.Text.Json;
using EBOS.CRM.IntegrationTests.Infrastructure;
using EBOS.CRM.IntegrationTests.TestUtils;
using FluentAssertions;

namespace EBOS.CRM.IntegrationTests.Customer360;

public class Customer360OperabilityEndpointsTest(CustomWebApplicationFactory factory)
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "OperationalReadiness");

    [Fact]
    public async Task Dashboard_And_Alerts_Require_Observability_Policy()
    {
        _client.DefaultRequestHeaders.Remove(TestAuthHandler.AuthModeHeader);
        _client.DefaultRequestHeaders.Remove(TestAuthHandler.RolesHeader);
        _client.DefaultRequestHeaders.Remove(TestAuthHandler.PermissionsHeader);

        var dashboard = await _client.GetAsync($"/api/v{_version}/OperationalReadiness/dashboard");
        dashboard.StatusCode.Should().Be(HttpStatusCode.Forbidden);

        _client.DefaultRequestHeaders.Add(TestAuthHandler.PermissionsHeader, "ops.observability.read");

        dashboard = await _client.GetAsync($"/api/v{_version}/OperationalReadiness/dashboard");
        dashboard.StatusCode.Should().Be(HttpStatusCode.OK);

        var dashboardBody = await dashboard.Content.ReadAsStringAsync();
        using var dashboardJson = JsonDocument.Parse(dashboardBody);
        dashboardJson.RootElement.TryGetProperty("snapshot", out _).Should().BeTrue();
        dashboardJson.RootElement.TryGetProperty("outboxPending", out _).Should().BeTrue();
        dashboardJson.RootElement.TryGetProperty("outboxFailed", out _).Should().BeTrue();

        var alerts = await _client.GetAsync($"/api/v{_version}/OperationalReadiness/alerts");
        alerts.StatusCode.Should().Be(HttpStatusCode.OK);

        var alertsBody = await alerts.Content.ReadAsStringAsync();
        using var alertsJson = JsonDocument.Parse(alertsBody);
        alertsJson.RootElement.TryGetProperty("outboxPendingWarning", out _).Should().BeTrue();
        alertsJson.RootElement.TryGetProperty("concurrencyCritical", out _).Should().BeTrue();
    }

    [Fact]
    public async Task Metrics_And_Ready_Health_Require_Auth_And_Policy()
    {
        _client.DefaultRequestHeaders.Remove(TestAuthHandler.AuthModeHeader);
        _client.DefaultRequestHeaders.Remove(TestAuthHandler.RolesHeader);
        _client.DefaultRequestHeaders.Remove(TestAuthHandler.PermissionsHeader);

        _client.DefaultRequestHeaders.Add(TestAuthHandler.AuthModeHeader, "none");
        var unauthorizedMetrics = await _client.GetAsync("/metrics");
        unauthorizedMetrics.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var unauthorizedReady = await _client.GetAsync("/health/ready");
        unauthorizedReady.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        _client.DefaultRequestHeaders.Remove(TestAuthHandler.AuthModeHeader);
        var forbiddenMetrics = await _client.GetAsync("/metrics");
        forbiddenMetrics.StatusCode.Should().Be(HttpStatusCode.Forbidden);

        var forbiddenReady = await _client.GetAsync("/health/ready");
        forbiddenReady.StatusCode.Should().Be(HttpStatusCode.Forbidden);

        _client.DefaultRequestHeaders.Add(TestAuthHandler.PermissionsHeader, "ops.observability.read,ops.readiness.read");
        var authorizedMetrics = await _client.GetAsync("/metrics");
        authorizedMetrics.StatusCode.Should().Be(HttpStatusCode.OK);

        var authorizedReady = await _client.GetAsync("/health/ready");
        authorizedReady.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.ServiceUnavailable);
    }

    [Fact]
    public async Task Live_Health_Remains_Exposed()
    {
        _client.DefaultRequestHeaders.Remove(TestAuthHandler.AuthModeHeader);
        _client.DefaultRequestHeaders.Remove(TestAuthHandler.RolesHeader);
        _client.DefaultRequestHeaders.Remove(TestAuthHandler.PermissionsHeader);

        _client.DefaultRequestHeaders.Add(TestAuthHandler.AuthModeHeader, "none");
        var live = await _client.GetAsync("/health/live");
        live.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.ServiceUnavailable);
    }
}
