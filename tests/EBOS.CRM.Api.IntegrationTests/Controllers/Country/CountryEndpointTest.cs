using System.Net;
using System.Net.Http.Json;
using EBOS.CRM.Api.IntegrationTests.Infrastructure;
using EBOS.CRM.Api.IntegrationTests.TestUtils;
using EBOS.CRM.Application.Contracts.Responses;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace EBOS.CRM.Api.IntegrationTests.Controllers.Country;

public class CountryEndpointTest(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "Country");

    [Fact]
    public async Task GetAll_Returns_Contract_Fields_With_Values()
    {
        var client = factory.CreateClient();
        var response = await client.GetAsync($"/api/v{_version}/Country");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var items = await response.Content.ReadItemsAsync<CountryResponse>();

        items.Should().NotBeEmpty();
        items.All(i => i.Id > 0).Should().BeTrue();
        items.All(i => !string.IsNullOrWhiteSpace(i.Name)).Should().BeTrue();
        items.All(i => !string.IsNullOrWhiteSpace(i.Iso31661A2Code)).Should().BeTrue();
        items.All(i => !string.IsNullOrWhiteSpace(i.Iso31661A3Code)).Should().BeTrue();
        items.All(i => !string.IsNullOrWhiteSpace(i.Iso31661NumCode)).Should().BeTrue();
        items.All(i => !string.IsNullOrWhiteSpace(i.Domain)).Should().BeTrue();
        items.All(i => !string.IsNullOrWhiteSpace(i.Currency)).Should().BeTrue();
        items.All(i => !string.IsNullOrWhiteSpace(i.CurrencyCode)).Should().BeTrue();
        items.All(i => !string.IsNullOrWhiteSpace(i.InternationalPhoneCode)).Should().BeTrue();
    }

    [Fact]
    public async Task GetAll_Supports_Pagination_And_Total_Header()
    {
        var client = factory.CreateClient();
        var response = await client.GetAsync($"/api/v{_version}/Country?pageNumber=1&pageSize=1");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Headers.Contains("X-Total-Count").Should().BeTrue();
        var total = int.Parse(response.Headers.GetValues("X-Total-Count").Single());
        total.Should().BeGreaterThanOrEqualTo(1);

        var items = await response.Content.ReadItemsAsync<CountryResponse>();
        items.Count.Should().Be(1);

        var responsePage2 = await client.GetAsync($"/api/v{_version}/Country?pageNumber=2&pageSize=1");
        responsePage2.StatusCode.Should().Be(HttpStatusCode.OK);
        var itemsPage2 = await responsePage2.Content.ReadItemsAsync<CountryResponse>();
        itemsPage2.Count.Should().Be(1);
    }

    [Fact]
    public async Task GetAll_Uses_Unique_Iso_Codes()
    {
        var (a2Code1, a2Code2, a3Code1, a3Code2) = SeedUniqueCountries();
        var client = factory.CreateClient();
        var response = await client.GetAsync($"/api/v{_version}/Country");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var items = await response.Content.ReadItemsAsync<CountryResponse>();

        items.Where(i => i.Iso31661A2Code == a2Code1 || i.Iso31661A2Code == a2Code2)
            .GroupBy(i => i.Iso31661A2Code)
            .All(g => g.Count() == 1)
            .Should().BeTrue();

        items.Where(i => i.Iso31661A3Code == a3Code1 || i.Iso31661A3Code == a3Code2)
            .GroupBy(i => i.Iso31661A3Code)
            .All(g => g.Count() == 1)
            .Should().BeTrue();
    }

    [Fact]
    public async Task GetAll_Contains_Seeded_Countries()
    {
        var expectedCodes = new[] { "ES", "FR", "DE", "IT" };
        var client = factory.CreateClient();
        var response = await client.GetAsync($"/api/v{_version}/Country");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var items = await response.Content.ReadItemsAsync<CountryResponse>();

        items.Select(i => i.Iso31661A2Code).Should().Contain(expectedCodes);
    }

    [Fact]
    public async Task GetById_Returns_Seeded_Item()
    {
        var client = factory.CreateClient();
        var listResponse = await client.GetAsync($"/api/v{_version}/Country");
        listResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var items = await listResponse.Content.ReadItemsAsync<CountryResponse>();
        var targetId = items.First().Id;

        var response = await client.GetAsync($"/api/v{_version}/Country/{targetId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var item = await response.Content.ReadFromJsonAsync<CountryResponse>();
        item.Should().NotBeNull();
        item.Id.Should().Be(targetId);
    }

    private (string A2Code1, string A2Code2, string A3Code1, string A3Code2) SeedUniqueCountries()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<EBOS.CRM.Infrastructure.Persistence.CrmDbContext>();

        var a2Code1 = $"X{Guid.NewGuid():N}"[..2].ToUpperInvariant();
        var a2Code2 = $"Y{Guid.NewGuid():N}"[..2].ToUpperInvariant();
        var a3Code1 = $"XA{Guid.NewGuid():N}"[..3].ToUpperInvariant();
        var a3Code2 = $"XB{Guid.NewGuid():N}"[..3].ToUpperInvariant();

        db.Countries.AddRange(
            new EBOS.CRM.Domain.Entities.Country
            {
                Name = $"Country-{a2Code1}",
                Iso31661A2Code = a2Code1,
                Iso31661A3Code = a3Code1,
                Iso31661NumCode = "901",
                Domain = "x1",
                Currency = "XCU",
                CurrencyCode = "XCU",
                InternationalPhoneCode = "+901"
            },
            new EBOS.CRM.Domain.Entities.Country
            {
                Name = $"Country-{a2Code2}",
                Iso31661A2Code = a2Code2,
                Iso31661A3Code = a3Code2,
                Iso31661NumCode = "902",
                Domain = "x2",
                Currency = "XCV",
                CurrencyCode = "XCV",
                InternationalPhoneCode = "+902"
            });

        db.SaveChanges();

        return (a2Code1, a2Code2, a3Code1, a3Code2);
    }

}
