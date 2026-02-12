using System.Net;
using System.Net.Http.Json;
using EBOS.CRM.Contracts.Responses.EBOS;
using EBOS.CRM.IntegrationTests.Infrastructure;
using EBOS.CRM.IntegrationTests.TestUtils;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace EBOS.CRM.IntegrationTests.Controllers.EBOS.Status;

public class StatusEndpointTest(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "Status");

    [Fact]
    public async Task GetAll_Returns_Contract_Fields_With_Values()
    {
        var client = factory.CreateClient();
        var response = await client.GetAsync($"/api/v{_version}/Status");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var items = await response.Content.ReadItemsAsync<StatusResponse>();

        items.Should().NotBeEmpty();
        items.All(i => i.Id > 0).Should().BeTrue();
        items.All(i => !string.IsNullOrWhiteSpace(i.Description)).Should().BeTrue();
    }

    [Fact]
    public async Task GetAll_Supports_Pagination_And_Total_Header()
    {
        var client = factory.CreateClient();
        var response = await client.GetAsync($"/api/v{_version}/Status?pageNumber=1&pageSize=1");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Headers.Contains("X-Total-Count").Should().BeTrue();
        var total = int.Parse(response.Headers.GetValues("X-Total-Count").Single());
        total.Should().BeGreaterThanOrEqualTo(1);

        var items = await response.Content.ReadItemsAsync<StatusResponse>();
        items.Count.Should().Be(1);

        var responsePage2 = await client.GetAsync($"/api/v{_version}/Status?pageNumber=2&pageSize=1");
        responsePage2.StatusCode.Should().Be(HttpStatusCode.OK);
        var itemsPage2 = await responsePage2.Content.ReadItemsAsync<StatusResponse>();
        itemsPage2.Count.Should().Be(1);
    }

    [Fact]
    public async Task GetAll_Uses_Unique_Descriptions()
    {
        var (desc1, desc2) = SeedUniqueStatuses();
        var client = factory.CreateClient();
        var response = await client.GetAsync($"/api/v{_version}/Status");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var items = await response.Content.ReadItemsAsync<StatusResponse>();

        items.Where(i => i.Description == desc1 || i.Description == desc2)
            .GroupBy(i => i.Description)
            .All(g => g.Count() == 1)
            .Should().BeTrue();
    }

    [Fact]
    public async Task GetAll_Contains_Seeded_Statuses()
    {
        var expectedDescriptions = new[] { "Active", "Inactive" };
        var client = factory.CreateClient();
        var response = await client.GetAsync($"/api/v{_version}/Status");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var items = await response.Content.ReadItemsAsync<StatusResponse>();

        items.Select(i => i.Description).Should().Contain(expectedDescriptions);
    }

    [Fact]
    public async Task GetById_Returns_Seeded_Item()
    {
        var client = factory.CreateClient();
        var listResponse = await client.GetAsync($"/api/v{_version}/Status");
        listResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var items = await listResponse.Content.ReadItemsAsync<StatusResponse>();
        var targetId = items.First().Id;

        var response = await client.GetAsync($"/api/v{_version}/Status/{targetId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var item = await response.Content.ReadFromJsonAsync<StatusResponse>();
        item.Should().NotBeNull();
        item.Id.Should().Be(targetId);
    }

    private (string Description1, string Description2) SeedUniqueStatuses()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<global::EBOS.CRM.Infrastructure.Persistence.CrmDbContext>();

        var desc1 = $"Status-{Guid.NewGuid():N}";
        var desc2 = $"Status-{Guid.NewGuid():N}";

        db.Statuses.AddRange(
            new Domain.Entities.EBOS.Status
            {
                Description = desc1
            },
            new Domain.Entities.EBOS.Status
            {
                Description = desc2
            });

        db.SaveChanges();

        return (desc1, desc2);
    }

}

