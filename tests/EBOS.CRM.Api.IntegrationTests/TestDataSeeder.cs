using EBOS.CRM.Domain.Entities;
using EBOS.CRM.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EBOS.CRM.Api.IntegrationTests;

public static class TestDataSeeder
{
    /// <summary>
    /// Seeder idempotente: comprueba existencia por Iso31661A2Code antes de insertar.
    /// Puede llamarse varias veces sin crear duplicados.
    /// </summary>
    public static async Task SeedCountriesAsync(CrmDbContext db)
    {
        // Lista base de países que queremos garantizar en la DB de pruebas
        var baseCountries = new[]
        {
            new { Name = "España", A2 = "ES", A3 = "ESP", Num = "724", Domain = "es", Currency = "Euro", CurrencyCode = "EUR", Phone = "+34" },
            new { Name = "Francia", A2 = "FR", A3 = "FRA", Num = "250", Domain = "fr", Currency = "Euro", CurrencyCode = "EUR", Phone = "+33" }
        };

        foreach (var c in baseCountries)
        {
            var exists = await db.Countries.AnyAsync(x => x.Iso31661A2Code == c.A2);
            if (!exists)
            {
                db.Countries.Add(new Country
                {
                    Name = c.Name,
                    Iso31661A2Code = c.A2,
                    Iso31661A3Code = c.A3,
                    Iso31661NumCode = c.Num,
                    Domain = c.Domain,
                    Currency = c.Currency,
                    CurrencyCode = c.CurrencyCode,
                    InternationalPhoneCode = c.Phone
                });
            }
        }

        await db.SaveChangesAsync();
    }
}