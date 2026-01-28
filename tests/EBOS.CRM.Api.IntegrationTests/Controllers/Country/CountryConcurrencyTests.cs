using System.Net;
using EBOS.CRM.Api.IntegrationTests.Infrastructure;
using FluentAssertions;

namespace EBOS.CRM.Api.IntegrationTests.Controllers.Country;

public class CountryConcurrencyTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Stress_GetAll_ConcurrentRequests_ReturnsConsistentResults()
    {
        var tasks = Enumerable.Range(0, 20)
            .Select(_ => _client.GetAsync("/api/v1/Country"))
            .ToList();

        var responses = await Task.WhenAll(tasks);

        responses.Should().OnlyContain(r => r.StatusCode == HttpStatusCode.OK);
    }

    [Fact]
    public async Task Stress_GetById_ConcurrentRequests_ReturnsConsistentResults()
    {
        var tasks = Enumerable.Range(0, 20)
            .Select(_ => _client.GetAsync("/api/v1/Country/1"))
            .ToList();

        var responses = await Task.WhenAll(tasks);

        responses.Should().OnlyContain(r => r.StatusCode == HttpStatusCode.OK);
    }
}
