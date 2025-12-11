using EBOS.CRM.Api.IntegrationTests;
using EBOS.CRM.Application.Features.Countries.Dtos;
using EBOS.CRM.Domain.Entities; // Si necesitas la entidad para eliminar; ajustar si procede
using EBOS.CRM.Infrastructure.Persistence; // Ajusta si tu DbContext está en otro namespace
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace EBOS.CRM.ApiTests.Controllers;

[Collection("IntegrationTestsCollection")]
public class CountriesControllerIntegrationTests
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;
    private static int _isoCounter = 0;

    public CountriesControllerIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));

        // Forzar creación del host y ejecución de ConfigureWebHost
        _client = _factory.CreateClient();

        // Asegurar que la base y el esquema existen
        _factory.EnsureDatabaseCreated();

        // Limpiar tabla para evitar colisiones entre tests
        TruncateCountries();
    }

    private static string CountriesUrl(string version) => $"/api/v{version}/Countries";

    /// <summary>
    /// Genera un código A2 único para evitar colisiones con índices únicos.
    /// </summary>
    private static string GenerateUniqueA2()
    {
        var n = Interlocked.Increment(ref _isoCounter);
        // Genera dos letras A-Z basadas en contador (ciclo)
        var a = (char)('A' + (n / 26) % 26);
        var b = (char)('A' + n % 26);
        return $"{a}{b}";
    }

    /// <summary>
    /// Seed que genera códigos únicos y reintenta en caso de SqlException por timing.
    /// </summary>
    private CountryResponseDto SeedCountryDto(string name)
    {
        var maxRetries = 10;
        var delayMs = 300;

        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                using var scope = _factory.Services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<CrmDbContext>();

                // Garantizar esquema
                try
                {
                    db.Database.Migrate();
                }
                catch
                {
                    db.Database.EnsureCreated();
                }

                var isoA2 = GenerateUniqueA2();

                var entity = new Country
                {
                    Name = name,
                    Iso31661A2Code = isoA2,
                    Iso31661A3Code = isoA2 + "X",
                    Iso31661NumCode = new Random().Next(100, 999).ToString(),
                    Domain = name.Length >= 2 ? name[..2].ToLower() : "xx",
                    Currency = "TestCoin",
                    CurrencyCode = "TST",
                    InternationalPhoneCode = "+00"
                };

                db.Countries.Add(entity);
                db.SaveChanges();

                return new CountryResponseDto(
                    Id: entity.Id,
                    Name: entity.Name,
                    Iso31661A2Code: entity.Iso31661A2Code,
                    Iso31661A3Code: entity.Iso31661A3Code,
                    Iso31661NumCode: entity.Iso31661NumCode,
                    Domain: entity.Domain,
                    Currency: entity.Currency,
                    CurrencyCode: entity.CurrencyCode,
                    InternationalPhoneCode: entity.InternationalPhoneCode
                );
            }
            catch (SqlException)
            {
                if (attempt == maxRetries) throw;
                Thread.Sleep(delayMs);
            }
        }

        throw new InvalidOperationException("No se pudo seedear la entidad tras varios reintentos.");
    }

    private void DeleteCountryById(long id)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CrmDbContext>();

        var entity = db.Countries.Find(id);
        if (entity != null)
        {
            db.Countries.Remove(entity);
            db.SaveChanges();
        }
    }

    /// <summary>
    /// Trunca la tabla Countries para dejar un estado limpio entre tests.
    /// </summary>
    private void TruncateCountries()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CrmDbContext>();

        // DELETE + reseed identity para evitar colisiones de PK
        db.Database.ExecuteSqlRaw("DELETE FROM dbo.Countries;");
        db.Database.ExecuteSqlRaw("DBCC CHECKIDENT('dbo.Countries', RESEED, 0);");
    }

    [Theory]
    [InlineData("1.0")]
    [InlineData("2.0")]
    public async Task GetById_Returns_Ok_When_Exists_For_Versions(string version)
    {
        CountryResponseDto seeded = default!;
        try
        {
            seeded = SeedCountryDto($"Seed-{version}-{Guid.NewGuid():N}");

            var resp = await _client.GetAsync($"{CountriesUrl(version)}/{seeded.Id}");

            resp.StatusCode.Should().Be(HttpStatusCode.OK);
            var dto = await resp.Content.ReadFromJsonAsync<CountryResponseDto>();
            dto.Should().NotBeNull();
            dto!.Id.Should().Be(seeded.Id);
            dto.Name.Should().Be(seeded.Name);
        }
        finally
        {
            if (seeded is not null) DeleteCountryById(seeded.Id);
        }
    }

    [Theory]
    [InlineData("1.0")]
    [InlineData("2.0")]
    public async Task GetById_Returns_NotFound_When_Missing_For_Versions(string version)
    {
        var id = long.MaxValue / 2;

        var resp = await _client.GetAsync($"{CountriesUrl(version)}/{id}");

        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var pd = await resp.Content.ReadFromJsonAsync<ProblemDetails>();
        pd.Should().NotBeNull();
        pd!.Detail.Should().Contain($"Country with id {id} not found.");
    }

    [Theory]
    [InlineData("1.0")]
    [InlineData("2.0")]
    public async Task GetAll_Returns_List_Including_Seeded_For_Versions(string version)
    {
        CountryResponseDto s1 = default!;
        CountryResponseDto s2 = default!;
        try
        {
            s1 = SeedCountryDto($"All-{version}-A-{Guid.NewGuid():N}");
            s2 = SeedCountryDto($"All-{version}-B-{Guid.NewGuid():N}");

            var resp = await _client.GetAsync(CountriesUrl(version));

            resp.StatusCode.Should().Be(HttpStatusCode.OK);
            var list = await resp.Content.ReadFromJsonAsync<CountryResponseDto[]>();
            list.Should().NotBeNull();
            list!.Select(x => x.Id).Should().Contain([s1.Id, s2.Id]);
        }
        finally
        {
            if (s1 is not null) DeleteCountryById(s1.Id);
            if (s2 is not null) DeleteCountryById(s2.Id);
        }
    }

    // Resto de tests adaptados de forma similar...
}