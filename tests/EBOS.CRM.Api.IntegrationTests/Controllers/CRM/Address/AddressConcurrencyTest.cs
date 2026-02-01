using System.Net;
using EBOS.CRM.Api.IntegrationTests.Infrastructure;
using EBOS.CRM.Api.IntegrationTests.TestUtils;
using FluentAssertions;

namespace EBOS.CRM.Api.IntegrationTests.Controllers.CRM.Address;

public class AddressConcurrencyTest : IClassFixture<InMemoryAddressWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly string _version;

    public AddressConcurrencyTest(InMemoryAddressWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        _version = ApiVersionHelper.GetLatestVersion(factory);

        if (!factory.Repository.Items.Any())
        {
            factory.Repository.AddAsync(new Domain.Entities.CRM.Address
            {
                Id = 1,
                Street = "Main St",
                ExternalNumber = "123",
                InternalNumber = null,
                BetweenStreet1 = null,
                BetweenStreet2 = null,
                Neighbourhood = "Center",
                City = "Quito",
                StateOrProvince = "Pichincha",
                PostalCode = "EC17001",
                GoogleMapsUrl = null,
                Latitude = 0,
                Longitude = 0,
                CountryId = 1,
                AddressTypeId = 1
            }).GetAwaiter().GetResult();
        }
    }

    [Fact]
    public async Task Stress_GetAll_ConcurrentRequests_ReturnsConsistentResults()
    {
        var tasks = Enumerable.Range(0, 20)
            .Select(_ => _client.GetAsync($"/api/{_version}/Address"))
            .ToList();

        var responses = await Task.WhenAll(tasks);

        responses.Should().OnlyContain(r => r.StatusCode == HttpStatusCode.OK);
    }

    [Fact]
    public async Task Stress_GetById_ConcurrentRequests_ReturnsConsistentResults()
    {
        var tasks = Enumerable.Range(0, 20)
            .Select(_ => _client.GetAsync($"/api/{_version}/Address/1"))
            .ToList();

        var responses = await Task.WhenAll(tasks);

        responses.Should().OnlyContain(r => r.StatusCode == HttpStatusCode.OK);
    }
}





