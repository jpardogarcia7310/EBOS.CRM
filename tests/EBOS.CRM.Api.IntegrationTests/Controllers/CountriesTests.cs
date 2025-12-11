using EBOS.CRM.Api.Controllers.Countries.Requests;
using EBOS.CRM.Application.Features.Countries.Dtos;
using EBOS.CRM.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace EBOS.CRM.Api.IntegrationTests.Controllers;

public class CountriesTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory = factory ?? throw new ArgumentNullException(nameof(factory));
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    private HttpClient CreateClient() => _factory.CreateClient();

    private async Task SeedAsync()
    {
        // Forzar creación del host y ejecución de ConfigureWebHost
        _ = _factory.CreateClient();

        // Asegurar que la base y el esquema existen
        _factory.EnsureDatabaseCreated();

        // Limpiar tabla para evitar duplicados entre ejecuciones
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<CrmDbContext>();
            await db.Database.ExecuteSqlRawAsync("DELETE FROM dbo.Countries;");
            await db.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT('dbo.Countries', RESEED, 0);");
        }

        // Ejecutar el seeder idempotente
        using var scope2 = _factory.Services.CreateScope();
        var db2 = scope2.ServiceProvider.GetRequiredService<CrmDbContext>();
        await TestDataSeeder.SeedCountriesAsync(db2);
    }

    [Fact]
    public async Task GetAll_Returns_ListOfCountries()
    {
        await SeedAsync();
        var client = CreateClient();

        var response = await client.GetAsync("/api/v1/Countries");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadFromJsonAsync<CountryResponseDto[]>(_jsonOptions);
        content.Should().NotBeNull();
        content!.Length.Should().BeGreaterThanOrEqualTo(2);
    }

    [Fact]
    public async Task GetById_Returns_Country_WhenExists()
    {
        await SeedAsync();
        var client = CreateClient();

        // Obtener lista para conocer un id
        var list = await client.GetFromJsonAsync<CountryResponseDto[]>("/api/v1/Countries", _jsonOptions);
        list.Should().NotBeNull();
        var id = list![0].Id;

        var response = await client.GetAsync($"/api/v1/Countries/{id}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var dto = await response.Content.ReadFromJsonAsync<CountryResponseDto>(_jsonOptions);
        dto.Should().NotBeNull();
        dto!.Id.Should().Be(id);
    }

    [Fact]
    public async Task GetById_Returns_404_WhenNotFound()
    {
        await SeedAsync();
        var client = CreateClient();

        var response = await client.GetAsync($"/api/v1/Countries/{int.MaxValue}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Add_Returns_CreatedCountry()
    {
        await SeedAsync();
        var client = CreateClient();

        var request = new AddCountryRQ(
            Name: "Portugal",
            Iso31661A2Code: "PT",
            Iso31661A3Code: "PRT",
            Iso31661NumCode: "620",
            Domain: "pt",
            Currency: "Euro",
            CurrencyCode: "EUR",
            InternationalPhoneCode: "+351"
        );

        var response = await client.PostAsJsonAsync("/api/v1/Countries", request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var dto = await response.Content.ReadFromJsonAsync<CountryResponseDto>(_jsonOptions);
        dto.Should().NotBeNull();
        dto!.Name.Should().Be("Portugal");
    }

    [Fact]
    public async Task Update_Returns_UpdatedCountry()
    {
        await SeedAsync();
        var client = CreateClient();

        var list = await client.GetFromJsonAsync<CountryResponseDto[]>("/api/v1/Countries", _jsonOptions);
        list.Should().NotBeNull();
        var id = list![0].Id;

        var update = new UpdateCountryRQ(
            Name: "España Updated",
            Iso31661A2Code: "ES",
            Iso31661A3Code: "ESP",
            Iso31661NumCode: "724",
            Domain: "es",
            Currency: "Euro",
            CurrencyCode: "EUR",
            InternationalPhoneCode: "+34"
        );

        var response = await client.PutAsJsonAsync($"/api/v1/Countries/{id}", update);
        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            var problems = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
            Console.WriteLine(JsonSerializer.Serialize(problems, new JsonSerializerOptions { WriteIndented = true }));
        }
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var updated = await response.Content.ReadFromJsonAsync<CountryResponseDto>(_jsonOptions);
        updated.Should().NotBeNull();
        updated!.Name.Should().Be("España Updated");
    }

    [Fact]
    public async Task Update_Returns_404_WhenNotFound()
    {
        await SeedAsync();
        var client = CreateClient();

        var update = new UpdateCountryRQ(
            Name: "No existe",
            Iso31661A2Code: "XX",
            Iso31661A3Code: "XXX",
            Iso31661NumCode: "999",
            Domain: "xx",
            Currency: "X",
            CurrencyCode: "XXX",
            InternationalPhoneCode: "+999"
        );

        var response = await client.PutAsJsonAsync($"/api/v1/Countries/{int.MaxValue}", update);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_Returns_NoContent_WhenDeleted()
    {
        await SeedAsync();
        var client = CreateClient();

        var list = await client.GetFromJsonAsync<CountryResponseDto[]>("/api/v1/Countries", _jsonOptions);
        list.Should().NotBeNull();
        var id = list![0].Id;

        var response = await client.DeleteAsync($"/api/v1/Countries/{id}");
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Confirmar que ya no existe
        var get = await client.GetAsync($"/api/v1/Countries/{id}");
        get.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_Returns_404_WhenNotFound()
    {
        await SeedAsync();
        var client = CreateClient();

        var response = await client.DeleteAsync($"/api/v1/Countries/{int.MaxValue}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Add_Returns_400_OnValidationError()
    {
        await SeedAsync();
        var client = CreateClient();

        // Name vacío -> validación
        var request = new AddCountryRQ(
            Name: "",
            Iso31661A2Code: "E",
            Iso31661A3Code: "ES",
            Iso31661NumCode: "0",
            Domain: "",
            Currency: "",
            CurrencyCode: "EU",
            InternationalPhoneCode: ""
        );

        var response = await client.PostAsJsonAsync("/api/v1/Countries", request);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Update_Returns_400_OnValidationError()
    {
        await SeedAsync();
        var client = CreateClient();

        var list = await client.GetFromJsonAsync<CountryResponseDto[]>("/api/v1/Countries", _jsonOptions);
        list.Should().NotBeNull();
        var id = list![0].Id;

        var update = new UpdateCountryRQ(
            Name: "",
            Iso31661A2Code: "E",
            Iso31661A3Code: "ES",
            Iso31661NumCode: "0",
            Domain: "",
            Currency: "",
            CurrencyCode: "EU",
            InternationalPhoneCode: ""
        );

        var response = await client.PutAsJsonAsync($"/api/v1/Countries/{id}", update);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}