using EBOS.CRM.Api.IntegrationTests.Infrastructure;
using EBOS.CRM.Api.IntegrationTests.TestUtils;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Json;

namespace EBOS.CRM.Api.IntegrationTests.Middleware;

public class ErrorHandlingTest(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory);

    [Fact]
    public async Task GetById_Returns_404_WhenCountryNotFound()
    {
        var response = await _client.GetAsync($"/api/v{_version}/Country/999999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problem.Should().NotBeNull();
        problem.Status.Should().Be(404);
        problem.Title.Should().Be("Resource not found");
    }

    [Fact]
    public async Task GetById_Returns_400_WhenIdIsInvalid()
    {
        // If your controller validates that the ID must be positive
        var response = await _client.GetAsync($"/api/v{_version}/Country/-1");
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problem.Should().NotBeNull();
        problem.Status.Should().Be(400);
        problem.Title.Should().Be("One or more validation errors occurred.");
    }
}







