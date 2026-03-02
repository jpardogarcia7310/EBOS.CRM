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
    public async Task Dashboard_And_Alerts_Endpoints_Return_Success()
    {
        var dashboard = await _client.GetAsync($"/api/v{_version}/OperationalReadiness/dashboard");
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
    public async Task Health_Endpoints_Are_Exposed()
    {
        var live = await _client.GetAsync("/health/live");
        live.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.ServiceUnavailable);

        var ready = await _client.GetAsync("/health/ready");
        ready.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.ServiceUnavailable);
    }
}
