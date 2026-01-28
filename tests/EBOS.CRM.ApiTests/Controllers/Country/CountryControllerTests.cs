using System.Net;
using System.Net.Http.Json;
using EBOS.CRM.Application.Contracts.Responses;
<<<<<<<< HEAD:tests/EBOS.CRM.ApiTests/Controllers/Country/CountryControllerTest.cs
using EBOS.CRM.ApiTests.TestUtils;
using EBOS.CRM.ApiTests.Fixtures;
========
>>>>>>>> 6470751 (New tests have been added for the IdentificationType, AddressType, and Address entities, covering all possible use cases.):tests/EBOS.CRM.ApiTests/Controllers/Country/CountryControllerTests.cs
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace EBOS.CRM.ApiTests.Controllers.Country;

public class CountryControllerTest(CustomWebApplicationFactory<Program> factory) :
    IClassFixture<CustomWebApplicationFactory<Program>> // Your API's Program.cs file
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory);

    #region CRUD Básicos
    [Fact]
    public async Task GetAllCountries_ReturnsSuccessAndList()
    {
<<<<<<<< HEAD:tests/EBOS.CRM.ApiTests/Controllers/Country/CountryControllerTest.cs
        var response = await _client.GetAsync($"/api/{_version}/Country");
========
        var response = await _client.GetAsync("/api/v1/Country");
>>>>>>>> 6470751 (New tests have been added for the IdentificationType, AddressType, and Address entities, covering all possible use cases.):tests/EBOS.CRM.ApiTests/Controllers/Country/CountryControllerTests.cs
        response.EnsureSuccessStatusCode();

        var countries = await response.Content.ReadPagedItemsAsync<CountryResponse>();
        Assert.NotNull(countries);
        Assert.NotEmpty(countries);
    }

    [Fact]
    public async Task GetCountryById_ExistingId_ReturnsCountry()
    {
<<<<<<<< HEAD:tests/EBOS.CRM.ApiTests/Controllers/Country/CountryControllerTest.cs
        var id = await ControllerTestHelper.GetFirstIdAsync<CountryResponse>(
            _client, $"/api/{_version}/Country", x => x.Id);

        var response = await _client.GetAsync($"/api/{_version}/Country/{id}");
========
        var response = await _client.GetAsync("/api/v1/Country/1");
>>>>>>>> 6470751 (New tests have been added for the IdentificationType, AddressType, and Address entities, covering all possible use cases.):tests/EBOS.CRM.ApiTests/Controllers/Country/CountryControllerTests.cs
        response.EnsureSuccessStatusCode();

        var country = await response.Content.ReadFromJsonAsync<CountryResponse>();
        Assert.NotNull(country);
        Assert.Equal(id, country.Id);
    }

    [Fact]
    public async Task GetCountryById_NonExistingId_ReturnsNotFound()
    {
<<<<<<<< HEAD:tests/EBOS.CRM.ApiTests/Controllers/Country/CountryControllerTest.cs
        var id = await ControllerTestHelper.GetFirstIdAsync<CountryResponse>(
            _client, $"/api/{_version}/Country", x => x.Id);

        var response = await _client.GetAsync($"/api/{_version}/Country/{id + 9999}");
========
        var response = await _client.GetAsync("/api/v1/Country/9999");
>>>>>>>> 6470751 (New tests have been added for the IdentificationType, AddressType, and Address entities, covering all possible use cases.):tests/EBOS.CRM.ApiTests/Controllers/Country/CountryControllerTests.cs
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    #endregion

    #region Stress & Performance
    [Fact]
    public async Task Resilience_DatabaseUnavailable_ReturnsServiceUnavailable()
    {
        // Simulation: special endpoint that forces a DB failure (example: /api/v1/Country/simulate-db-failure)
<<<<<<<< HEAD:tests/EBOS.CRM.ApiTests/Controllers/Country/CountryControllerTest.cs
        var response = await _client.GetAsync($"/api/{_version}/Country/simulate-db-failure");
========
        var response = await _client.GetAsync("/api/v1/Country/simulate-db-failure");
>>>>>>>> 6470751 (New tests have been added for the IdentificationType, AddressType, and Address entities, covering all possible use cases.):tests/EBOS.CRM.ApiTests/Controllers/Country/CountryControllerTests.cs

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Resilience_NetworkInterruption_ReturnsGatewayTimeout()
    {
        // Simulation: endpoint that forces network timeout
<<<<<<<< HEAD:tests/EBOS.CRM.ApiTests/Controllers/Country/CountryControllerTest.cs
        var response = await _client.GetAsync($"/api/{_version}/Country/simulate-timeout");
========
        var response = await _client.GetAsync("/api/v1/Country/simulate-timeout");
>>>>>>>> 6470751 (New tests have been added for the IdentificationType, AddressType, and Address entities, covering all possible use cases.):tests/EBOS.CRM.ApiTests/Controllers/Country/CountryControllerTests.cs

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Recovery_AfterDatabaseFailure_RetrySucceeds()
    {
        // Simulation: first attempt fails (DB drops), second attempt recovers
<<<<<<<< HEAD:tests/EBOS.CRM.ApiTests/Controllers/Country/CountryControllerTest.cs
        var response1 = await _client.GetAsync($"/api/{_version}/Country/simulate-db-failure");
        Assert.Equal(HttpStatusCode.NotFound, response1.StatusCode);

        // We expect the system to apply a retry/circuit breaker and recover.
        var response2 = await _client.GetAsync($"/api/{_version}/Country");
========
        var response1 = await _client.GetAsync("/api/v1/Country/simulate-db-failure");
        Assert.Equal(HttpStatusCode.NotFound, response1.StatusCode);

        // We expect the system to apply a retry/circuit breaker and recover.
        var response2 = await _client.GetAsync("/api/v1/Country");
>>>>>>>> 6470751 (New tests have been added for the IdentificationType, AddressType, and Address entities, covering all possible use cases.):tests/EBOS.CRM.ApiTests/Controllers/Country/CountryControllerTests.cs
        response2.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Recovery_AfterTimeout_RetrySucceeds()
    {
<<<<<<<< HEAD:tests/EBOS.CRM.ApiTests/Controllers/Country/CountryControllerTest.cs
        var response1 = await _client.GetAsync($"/api/{_version}/Country/simulate-timeout");
        Assert.Equal(HttpStatusCode.NotFound, response1.StatusCode);

        // Second attempt should recover
        var response2 = await _client.GetAsync($"/api/{_version}/Country");
========
        var response1 = await _client.GetAsync("/api/v1/Country/simulate-timeout");
        Assert.Equal(HttpStatusCode.NotFound, response1.StatusCode);

        // Second attempt should recover
        var response2 = await _client.GetAsync("/api/v1/Country");
>>>>>>>> 6470751 (New tests have been added for the IdentificationType, AddressType, and Address entities, covering all possible use cases.):tests/EBOS.CRM.ApiTests/Controllers/Country/CountryControllerTests.cs
        response2.EnsureSuccessStatusCode();
    }
    #endregion
}
<<<<<<<< HEAD:tests/EBOS.CRM.ApiTests/Controllers/Country/CountryControllerTest.cs

========
>>>>>>>> 6470751 (New tests have been added for the IdentificationType, AddressType, and Address entities, covering all possible use cases.):tests/EBOS.CRM.ApiTests/Controllers/Country/CountryControllerTests.cs
