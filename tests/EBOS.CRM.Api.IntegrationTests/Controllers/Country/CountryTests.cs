using System.Net;
using System.Net.Http.Json;
using EBOS.CRM.Api.IntegrationTests.Infrastructure;
using EBOS.CRM.Api.IntegrationTests.TestUtils;
using EBOS.CRM.Application.Contracts.Responses;
using FluentAssertions;

namespace EBOS.CRM.Api.IntegrationTests.Controllers.Country;

<<<<<<<< HEAD:tests/EBOS.CRM.Api.IntegrationTests/Controllers/Country/CountryTest.cs
public class CountryTest(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
========
public class CountryTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
>>>>>>>> 6470751 (New tests have been added for the IdentificationType, AddressType, and Address entities, covering all possible use cases.):tests/EBOS.CRM.Api.IntegrationTests/Controllers/Country/CountryTests.cs
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory);

    [Fact]
    public async Task GetAll_Returns_ListOfItems()
    {
<<<<<<<< HEAD:tests/EBOS.CRM.Api.IntegrationTests/Controllers/Country/CountryTest.cs
        var response = await _client.GetAsync($"/api/{_version}/Country");
========
        var response = await _client.GetAsync("/api/v1/Country");
>>>>>>>> 6470751 (New tests have been added for the IdentificationType, AddressType, and Address entities, covering all possible use cases.):tests/EBOS.CRM.Api.IntegrationTests/Controllers/Country/CountryTests.cs
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var countries = await response.Content.ReadPagedItemsAsync<CountryResponse>();
        countries.Should().NotBeNull();
        countries.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetById_Returns_Country_WhenExists()
    {
<<<<<<<< HEAD:tests/EBOS.CRM.Api.IntegrationTests/Controllers/Country/CountryTest.cs
        var response = await _client.GetAsync($"/api/{_version}/Country/1");
========
        var response = await _client.GetAsync("/api/v1/Country/1");
>>>>>>>> 6470751 (New tests have been added for the IdentificationType, AddressType, and Address entities, covering all possible use cases.):tests/EBOS.CRM.Api.IntegrationTests/Controllers/Country/CountryTests.cs
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var country = await response.Content.ReadFromJsonAsync<CountryResponse>();
        country.Should().NotBeNull();
        country.Name.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetById_Returns_404_WhenNotFound()
    {
<<<<<<<< HEAD:tests/EBOS.CRM.Api.IntegrationTests/Controllers/Country/CountryTest.cs
        var response = await _client.GetAsync($"/api/{_version}/Country/999999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}






========
        var response = await _client.GetAsync("/api/v1/Country/999999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
>>>>>>>> 6470751 (New tests have been added for the IdentificationType, AddressType, and Address entities, covering all possible use cases.):tests/EBOS.CRM.Api.IntegrationTests/Controllers/Country/CountryTests.cs
