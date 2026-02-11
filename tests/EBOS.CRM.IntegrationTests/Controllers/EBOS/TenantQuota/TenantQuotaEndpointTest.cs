using System.Net;
using System.Net.Http.Json;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Application.Contracts.Responses.EBOS;
using EBOS.CRM.IntegrationTests.Infrastructure;
using EBOS.CRM.IntegrationTests.TestUtils;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace EBOS.CRM.Api.IntegrationTests.Controllers.EBOS.TenantQuota;

public class TenantQuotaEndpointTest(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "TenantQuota");

    [Fact]
    public async Task GetAll_Returns_Items_And_Total_Header()
    {
        var client = factory.CreateClient();
        var response = await client.GetAsync($"/api/v{_version}/TenantQuota?pageNumber=1&pageSize=1");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Headers.Contains("X-Total-Count").Should().BeTrue();
        var items = await response.Content.ReadItemsAsync<TenantQuotaResponse>();
        items.Count.Should().Be(1);
    }

    [Fact]
    public async Task GetById_Returns_Seeded_Item()
    {
        var client = factory.CreateClient();
        var listResponse = await client.GetAsync($"/api/v{_version}/TenantQuota");
        listResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var items = await listResponse.Content.ReadItemsAsync<TenantQuotaResponse>();
        var targetId = items.First().Id;

        var response = await client.GetAsync($"/api/v{_version}/TenantQuota/{targetId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var item = await response.Content.ReadFromJsonAsync<TenantQuotaResponse>();
        item.Should().NotBeNull();
        item!.Id.Should().Be(targetId);
    }

    [Fact]
    public async Task GetAll_Contains_Unique_Metrics()
    {
        var (metric1, metric2) = SeedUniqueQuotas();
        var client = factory.CreateClient();
        var response = await client.GetAsync($"/api/v{_version}/TenantQuota");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var items = await response.Content.ReadItemsAsync<TenantQuotaResponse>();

        items.Where(i => i.Metric == metric1 || i.Metric == metric2)
            .GroupBy(i => i.Metric)
            .All(g => g.Count() == 1)
            .Should().BeTrue();
    }

    private (string Metric1, string Metric2) SeedUniqueQuotas()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<global::EBOS.CRM.Infrastructure.Persistence.CrmDbContext>();

        var metric1 = $"users.{Guid.NewGuid():N}"[..10];
        var metric2 = $"api.{Guid.NewGuid():N}"[..10];

        db.TenantQuotas.AddRange(
            new Domain.Entities.EBOS.TenantQuota
            {
                TenantId = 1,
                Metric = metric1,
                Limit = 100,
                Unit = "count",
                EffectiveFrom = DateTime.UtcNow.AddDays(-1)
            },
            new Domain.Entities.EBOS.TenantQuota
            {
                TenantId = 1,
                Metric = metric2,
                Limit = 1000,
                Unit = "count",
                EffectiveFrom = DateTime.UtcNow.AddDays(-1)
            });

        db.SaveChanges();

        return (metric1, metric2);
    }
}

