using System.Net;
using System.Net.Http.Json;
using EBOS.CRM.Application.Contracts.Responses;
using EBOS.CRM.IntegrationTests.Infrastructure;
using EBOS.CRM.IntegrationTests.TestUtils;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace EBOS.CRM.IntegrationTests.Controllers.EBOS.IdentificationType;

public class IdentificationTypeEndpointTest(CustomWebApplicationFactory factory)
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "IdentificationType");

    [Fact]
    public async Task GetAll_Returns_Contract_Fields_With_Values()
    {
        var client = factory.CreateClient();
        var response = await client.GetAsync($"/api/v{_version}/IdentificationType");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var items = await response.Content.ReadItemsAsync<IdentificationTypeResponse>();

        items.Should().NotBeEmpty();
        items.All(i => i.Id > 0).Should().BeTrue();
        items.All(i => !string.IsNullOrWhiteSpace(i.Code)).Should().BeTrue();
        items.All(i => !string.IsNullOrWhiteSpace(i.Description)).Should().BeTrue();
    }

    [Fact]
    public async Task GetAll_Supports_Pagination_And_Total_Header()
    {
        var client = factory.CreateClient();
        var response = await client.GetAsync($"/api/v{_version}/IdentificationType?pageNumber=1&pageSize=1");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Headers.Contains("X-Total-Count").Should().BeTrue();
        var total = int.Parse(response.Headers.GetValues("X-Total-Count").Single());
        total.Should().BeGreaterThanOrEqualTo(1);

        var items = await response.Content.ReadItemsAsync<IdentificationTypeResponse>();
        items.Count.Should().Be(1);

        var responsePage2 = await client.GetAsync($"/api/v{_version}/IdentificationType?pageNumber=2&pageSize=1");
        responsePage2.StatusCode.Should().Be(HttpStatusCode.OK);
        var itemsPage2 = await responsePage2.Content.ReadItemsAsync<IdentificationTypeResponse>();
        itemsPage2.Count.Should().Be(1);
    }

    [Fact]
    public async Task GetAll_Uses_Unique_Codes()
    {
        var (code1, code2) = SeedUniqueIdentificationTypes();
        var client = factory.CreateClient();
        var response = await client.GetAsync($"/api/v{_version}/IdentificationType");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var items = await response.Content.ReadItemsAsync<IdentificationTypeResponse>();

        items.Where(i => i.Code == code1 || i.Code == code2)
            .GroupBy(i => i.Code)
            .All(g => g.Count() == 1)
            .Should().BeTrue();
    }

    [Fact]
    public async Task GetAll_Contains_Seeded_Identification_Types()
    {
        var expectedCodes = new[] { "DNI", "PASS" };
        var client = factory.CreateClient();
        var response = await client.GetAsync($"/api/v{_version}/IdentificationType");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var items = await response.Content.ReadItemsAsync<IdentificationTypeResponse>();

        items.Select(i => i.Code).Should().Contain(expectedCodes);
    }

    [Fact]
    public async Task GetById_Returns_Seeded_Item()
    {
        var client = factory.CreateClient();
        var listResponse = await client.GetAsync($"/api/v{_version}/IdentificationType");
        listResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var items = await listResponse.Content.ReadItemsAsync<IdentificationTypeResponse>();
        var targetId = items.First().Id;

        var response = await client.GetAsync($"/api/v{_version}/IdentificationType/{targetId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var item = await response.Content.ReadFromJsonAsync<IdentificationTypeResponse>();
        item.Should().NotBeNull();
        item.Id.Should().Be(targetId);
    }

    private (string Code1, string Code2) SeedUniqueIdentificationTypes()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<global::EBOS.CRM.Infrastructure.Persistence.CrmDbContext>();

        var code1 = $"ID{Guid.NewGuid():N}"[..5].ToUpperInvariant();
        var code2 = $"JD{Guid.NewGuid():N}"[..5].ToUpperInvariant();

        db.IdentificationTypes.AddRange(
            new Domain.Entities.IdentificationType
            {
                Code = code1,
                Description = $"Desc-{code1}"
            },
            new Domain.Entities.IdentificationType
            {
                Code = code2,
                Description = $"Desc-{code2}"
            });

        db.SaveChanges();

        return (code1, code2);
    }

}

