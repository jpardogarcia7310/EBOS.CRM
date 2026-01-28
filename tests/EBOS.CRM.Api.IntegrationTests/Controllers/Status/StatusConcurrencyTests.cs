using System.Net;
using EBOS.CRM.Api.IntegrationTests.Infrastructure;
using EBOS.CRM.Api.IntegrationTests.TestUtils;
using FluentAssertions;

<<<<<<<< HEAD:tests/EBOS.CRM.Api.IntegrationTests/Controllers/Country/CountryConcurrencyTest.cs
namespace EBOS.CRM.Api.IntegrationTests.Controllers.Country;

public class CountryConcurrencyTest(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
========
namespace EBOS.CRM.Api.IntegrationTests.Controllers.Status;

public class StatusConcurrencyTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
>>>>>>>> 6470751 (New tests have been added for the IdentificationType, AddressType, and Address entities, covering all possible use cases.):tests/EBOS.CRM.Api.IntegrationTests/Controllers/Status/StatusConcurrencyTests.cs
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory);

    [Fact]
    public async Task Stress_GetAll_ConcurrentRequests_ReturnsConsistentResults()
    {
        var tasks = Enumerable.Range(0, 20)
<<<<<<<< HEAD:tests/EBOS.CRM.Api.IntegrationTests/Controllers/Country/CountryConcurrencyTest.cs
            .Select(_ => _client.GetAsync($"/api/{_version}/Country"))
========
            .Select(_ => _client.GetAsync("/api/v1/Status"))
>>>>>>>> 6470751 (New tests have been added for the IdentificationType, AddressType, and Address entities, covering all possible use cases.):tests/EBOS.CRM.Api.IntegrationTests/Controllers/Status/StatusConcurrencyTests.cs
            .ToList();

        var responses = await Task.WhenAll(tasks);

        responses.Should().OnlyContain(r => r.StatusCode == HttpStatusCode.OK);
    }

    [Fact]
    public async Task Stress_GetById_ConcurrentRequests_ReturnsConsistentResults()
    {
        var tasks = Enumerable.Range(0, 20)
<<<<<<<< HEAD:tests/EBOS.CRM.Api.IntegrationTests/Controllers/Country/CountryConcurrencyTest.cs
            .Select(_ => _client.GetAsync($"/api/{_version}/Country/1"))
========
            .Select(_ => _client.GetAsync("/api/v1/Status/1"))
>>>>>>>> 6470751 (New tests have been added for the IdentificationType, AddressType, and Address entities, covering all possible use cases.):tests/EBOS.CRM.Api.IntegrationTests/Controllers/Status/StatusConcurrencyTests.cs
            .ToList();

        var responses = await Task.WhenAll(tasks);

        responses.Should().OnlyContain(r => r.StatusCode == HttpStatusCode.OK);
    }
}
<<<<<<<< HEAD:tests/EBOS.CRM.Api.IntegrationTests/Controllers/Country/CountryConcurrencyTest.cs





========
>>>>>>>> 6470751 (New tests have been added for the IdentificationType, AddressType, and Address entities, covering all possible use cases.):tests/EBOS.CRM.Api.IntegrationTests/Controllers/Status/StatusConcurrencyTests.cs
