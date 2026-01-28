using System.Net;
using EBOS.CRM.Api.IntegrationTests.Infrastructure;
using EBOS.CRM.Api.IntegrationTests.TestUtils;
using FluentAssertions;

<<<<<<<< HEAD:tests/EBOS.CRM.Api.IntegrationTests/Controllers/Status/StatusConcurrencyTest.cs
namespace EBOS.CRM.Api.IntegrationTests.Controllers.Status;

public class StatusConcurrencyTest(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
========
namespace EBOS.CRM.Api.IntegrationTests.Controllers.Country;

public class CountryConcurrencyTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
>>>>>>>> 6470751 (New tests have been added for the IdentificationType, AddressType, and Address entities, covering all possible use cases.):tests/EBOS.CRM.Api.IntegrationTests/Controllers/Country/CountryConcurrencyTests.cs
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory);

    [Fact]
    public async Task Stress_GetAll_ConcurrentRequests_ReturnsConsistentResults()
    {
        var tasks = Enumerable.Range(0, 20)
<<<<<<<< HEAD:tests/EBOS.CRM.Api.IntegrationTests/Controllers/Status/StatusConcurrencyTest.cs
            .Select(_ => _client.GetAsync($"/api/{_version}/Status"))
========
            .Select(_ => _client.GetAsync("/api/v1/Country"))
>>>>>>>> 6470751 (New tests have been added for the IdentificationType, AddressType, and Address entities, covering all possible use cases.):tests/EBOS.CRM.Api.IntegrationTests/Controllers/Country/CountryConcurrencyTests.cs
            .ToList();

        var responses = await Task.WhenAll(tasks);

        responses.Should().OnlyContain(r => r.StatusCode == HttpStatusCode.OK);
    }

    [Fact]
    public async Task Stress_GetById_ConcurrentRequests_ReturnsConsistentResults()
    {
        var tasks = Enumerable.Range(0, 20)
<<<<<<<< HEAD:tests/EBOS.CRM.Api.IntegrationTests/Controllers/Status/StatusConcurrencyTest.cs
            .Select(_ => _client.GetAsync($"/api/{_version}/Status/1"))
========
            .Select(_ => _client.GetAsync("/api/v1/Country/1"))
>>>>>>>> 6470751 (New tests have been added for the IdentificationType, AddressType, and Address entities, covering all possible use cases.):tests/EBOS.CRM.Api.IntegrationTests/Controllers/Country/CountryConcurrencyTests.cs
            .ToList();

        var responses = await Task.WhenAll(tasks);

        responses.Should().OnlyContain(r => r.StatusCode == HttpStatusCode.OK);
    }
}
<<<<<<<< HEAD:tests/EBOS.CRM.Api.IntegrationTests/Controllers/Status/StatusConcurrencyTest.cs





========
>>>>>>>> 6470751 (New tests have been added for the IdentificationType, AddressType, and Address entities, covering all possible use cases.):tests/EBOS.CRM.Api.IntegrationTests/Controllers/Country/CountryConcurrencyTests.cs
