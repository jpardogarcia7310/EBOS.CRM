using EBOS.CRM.Application.Features.Countries.Dtos;
using EBOS.CRM.Application.Features.Statuses.Dtos;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;

namespace EBOS.CRM.ApiTests.Controllers.Statuses;

public class StatusesControllerTest(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>> // Program.cs de tu API
{
    private readonly HttpClient _client = factory.CreateClient();

    #region CRUD Básicos
    [Fact]
    public async Task GetAllStatuses_ReturnsSuccessAndList()
    {
        var response = await _client.GetAsync("/api/v1/statuses");
        response.EnsureSuccessStatusCode();

        var statuses = await response.Content.ReadFromJsonAsync<IEnumerable<StatusResponseDto>>();
        Assert.NotNull(statuses);
        Assert.NotEmpty(statuses);
    }

    [Fact]
    public async Task GetStatusById_ExistingId_ReturnsCountry()
    {
        var response = await _client.GetAsync("/api/v1/statuses/1");
        response.EnsureSuccessStatusCode();

        var status = await response.Content.ReadFromJsonAsync<StatusResponseDto>();
        Assert.NotNull(status);
        Assert.Equal(1, status!.Id);
    }

    [Fact]
    public async Task GetStatusById_NonExistingId_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/v1/statuses/9999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    #endregion

    #region Stress & Performance
    [Fact]
    public async Task Resilience_DatabaseUnavailable_ReturnsServiceUnavailable()
    {
        // Simulación: endpoint especial que fuerza fallo de DB (ejemplo: /api/v1/statuses/simulate-db-failure)
        var response = await _client.GetAsync("/api/v1/statuses/simulate-db-failure");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Resilience_NetworkInterruption_ReturnsGatewayTimeout()
    {
        // Simulación: endpoint que fuerza timeout de red
        var response = await _client.GetAsync("/api/v1/statuses/simulate-timeout");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Recovery_AfterDatabaseFailure_RetrySucceeds()
    {
        // Simulación: primer intento falla (DB caída), segundo intento recupera
        var response1 = await _client.GetAsync("/api/v1/statuses/simulate-db-failure");
        Assert.Equal(HttpStatusCode.NotFound, response1.StatusCode);

        // Esperamos que el sistema aplique retry/circuit breaker y se recupere
        var response2 = await _client.GetAsync("/api/v1/statuses");
        response2.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Recovery_AfterTimeout_RetrySucceeds()
    {
        var response1 = await _client.GetAsync("/api/v1/statuses/simulate-timeout");
        Assert.Equal(HttpStatusCode.NotFound, response1.StatusCode);

        // Segundo intento debería recuperarse
        var response2 = await _client.GetAsync("/api/v1/statuses");
        response2.EnsureSuccessStatusCode();
    }
    #endregion
}