using EBOS.CRM.Domain.Entities;
using EBOS.CRM.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EBOS.CRM.Api.IntegrationTests.Infrastructure;

public static class TestDataSeeder
{
    /// <summary>
    /// Idempotent seeder: checks for existence by Iso31661A2Code before inserting.
    /// Add pre-validation and exception capture to expose the real cause.
    /// </summary>
    public static async Task SeedCountriesAsync(CrmDbContext db)
    {
        var baseCountries = new[]
        {
            new { Name = "España", A2 = "ES", A3 = "ESP", Num = "724", Domain = "es", Currency = "Euro", CurrencyCode = "EUR", Phone = "+34" },
            new { Name = "Francia", A2 = "FR", A3 = "FRA", Num = "250", Domain = "fr", Currency = "Euro", CurrencyCode = "EUR", Phone = "+33" },
            new { Name = "Alemania", A2 = "DE", A3 = "DEU", Num = "276", Domain = "de", Currency = "Euro", CurrencyCode = "EUR", Phone = "+49" },
            new { Name = "Italia", A2 = "IT", A3 = "ITA", Num = "380", Domain = "it", Currency = "Euro", CurrencyCode = "EUR", Phone = "+39" }
        };

        foreach (var c in baseCountries)
        {
            var exists = await db.Countries.AnyAsync(x => x.Iso31661A2Code == c.A2);
            if (exists) continue;
            // Prior validation to avoid failed SaveChanges and provide clear messages
            var validation = ValidateSeedRow(c.Name, c.A2, c.A3, c.Num, c.Domain, c.CurrencyCode);
            if (validation.Any())
            {
                throw new InvalidOperationException(
                    $"Seed data contains invalid country entries. Errors: {string.Join("; ", validation)}");
            }

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

        try
        {
            await db.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                "Seed data contains invalid country entries. Inner exception: " + ex.Message, ex);
        }
    }

    private static IEnumerable<string> ValidateSeedRow(string name, string a2, string a3, string num, string domain, string currencyCode)
    {
        if (string.IsNullOrWhiteSpace(name)) yield return "Name empty";
        if (string.IsNullOrWhiteSpace(a2) || a2.Length != 2) yield return "Iso31661A2Code must be 2 letters";
        if (string.IsNullOrWhiteSpace(a3) || a3.Length != 3) yield return "Iso31661A3Code must be 3 letters";
        if (string.IsNullOrWhiteSpace(num) || num.Length < 3) yield return "Iso31661NumCode must be at least 3 digits";
        if (string.IsNullOrWhiteSpace(domain) || domain.Length < 2) yield return "Domain must be at least 2 chars";
        if (string.IsNullOrWhiteSpace(currencyCode) || currencyCode.Length != 3) yield return "CurrencyCode must be 3 letters";
    }
}
