using System.Net;
using System.Net.Http.Json;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.IntegrationTests.Infrastructure;
using EBOS.CRM.IntegrationTests.TestUtils;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace EBOS.CRM.Api.IntegrationTests.Controllers.EBOS.TenantUsageMetric;

public class TenantUsageMetricEndpointTest(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "TenantUsageMetric");

    [Fact]
    public async Task GetAll_Returns_Items_And_Total_Header()
    {
        var client = factory.CreateClient();
        var response = await client.GetAsync($"/api/v{_version}/TenantUsageMetric?pageNumber=1&pageSize=1");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Headers.Contains("X-Total-Count").Should().BeTrue();
        var items = await response.Content.ReadItemsAsync<TenantUsageMetricResponse>();
        items.Count.Should().Be(1);
    }

    [Fact]
    public async Task GetById_Returns_Seeded_Item()
    {
        var client = factory.CreateClient();
        var listResponse = await client.GetAsync($"/api/v{_version}/TenantUsageMetric");
        listResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var items = await listResponse.Content.ReadItemsAsync<TenantUsageMetricResponse>();
        var targetId = items.First().Id;

        var response = await client.GetAsync($"/api/v{_version}/TenantUsageMetric/{targetId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var item = await response.Content.ReadFromJsonAsync<TenantUsageMetricResponse>();
        item.Should().NotBeNull();
        item!.Id.Should().Be(targetId);
    }

    [Fact]
    public async Task GetAll_Contains_Unique_Metrics()
    {
        var (metric1, metric2) = SeedUniqueUsageMetrics();
        var client = factory.CreateClient();
        var response = await client.GetAsync($"/api/v{_version}/TenantUsageMetric");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var items = await response.Content.ReadItemsAsync<TenantUsageMetricResponse>();

        items.Where(i => i.Metric == metric1 || i.Metric == metric2)
            .GroupBy(i => i.Metric)
            .All(g => g.Count() == 1)
            .Should().BeTrue();
    }

    private (string Metric1, string Metric2) SeedUniqueUsageMetrics()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<global::EBOS.CRM.Infrastructure.Persistence.CrmDbContext>();

        var metric1 = $"calls.{Guid.NewGuid():N}"[..10];
        var metric2 = $"storage.{Guid.NewGuid():N}"[..10];

        db.TenantUsageMetrics.AddRange(
            new Domain.Entities.CRM.TenantUsageMetric
            {
                TenantId = 1,
                Metric = metric1,
                Value = 250,
                Unit = "count",
                PeriodStart = DateTime.UtcNow.AddDays(-7),
                PeriodEnd = DateTime.UtcNow,
                Source = "gateway"
            },
            new Domain.Entities.CRM.TenantUsageMetric
            {
                TenantId = 1,
                Metric = metric2,
                Value = 10,
                Unit = "gb",
                PeriodStart = DateTime.UtcNow.AddDays(-7),
                PeriodEnd = DateTime.UtcNow,
                Source = "metrics"
            });

        db.SaveChanges();

        return (metric1, metric2);
    }
}

