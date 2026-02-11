using System.Net;
using System.Net.Http.Json;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Application.Contracts.Responses.EBOS;
using EBOS.CRM.IntegrationTests.Infrastructure;
using EBOS.CRM.IntegrationTests.TestUtils;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace EBOS.CRM.Api.IntegrationTests.Controllers.EBOS.TenantConfiguration;

public class TenantConfigurationEndpointTest(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "TenantConfiguration");

    [Fact]
    public async Task GetAll_Returns_Items_And_Total_Header()
    {
        var client = factory.CreateClient();
        var response = await client.GetAsync($"/api/v{_version}/TenantConfiguration?pageNumber=1&pageSize=1");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Headers.Contains("X-Total-Count").Should().BeTrue();
        var items = await response.Content.ReadItemsAsync<TenantConfigurationResponse>();
        items.Count.Should().Be(1);
    }

    [Fact]
    public async Task GetById_Returns_Seeded_Item()
    {
        var client = factory.CreateClient();
        var listResponse = await client.GetAsync($"/api/v{_version}/TenantConfiguration");
        listResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var items = await listResponse.Content.ReadItemsAsync<TenantConfigurationResponse>();
        var targetId = items.First().Id;

        var response = await client.GetAsync($"/api/v{_version}/TenantConfiguration/{targetId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var item = await response.Content.ReadFromJsonAsync<TenantConfigurationResponse>();
        item.Should().NotBeNull();
        item!.Id.Should().Be(targetId);
    }

    [Fact]
    public async Task GetAll_Contains_Unique_Keys()
    {
        var (key1, key2) = SeedUniqueConfigurations();
        var client = factory.CreateClient();
        var response = await client.GetAsync($"/api/v{_version}/TenantConfiguration");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var items = await response.Content.ReadItemsAsync<TenantConfigurationResponse>();

        items.Where(i => i.Key == key1 || i.Key == key2)
            .GroupBy(i => i.Key)
            .All(g => g.Count() == 1)
            .Should().BeTrue();
    }

    private (string Key1, string Key2) SeedUniqueConfigurations()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<global::EBOS.CRM.Infrastructure.Persistence.CrmDbContext>();

        var key1 = $"cfg.{Guid.NewGuid():N}"[..10];
        var key2 = $"cfg.{Guid.NewGuid():N}"[..10];

        db.TenantConfigurations.AddRange(
            new Domain.Entities.EBOS.TenantConfiguration
            {
                TenantId = 1,
                Key = key1,
                ValueJson = "{\"value\":true}",
                UpdatedAt = DateTime.UtcNow,
                UpdatedBy = 1
            },
            new Domain.Entities.EBOS.TenantConfiguration
            {
                TenantId = 1,
                Key = key2,
                ValueJson = "{\"value\":false}",
                UpdatedAt = DateTime.UtcNow,
                UpdatedBy = 1
            });

        db.SaveChanges();

        return (key1, key2);
    }
}

