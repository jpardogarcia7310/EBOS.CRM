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

    public static async Task SeedAddressTypesAsync(CrmDbContext db)
    {
        if (await db.AddressTypes.AnyAsync())
        {
            return;
        }

        db.AddressTypes.AddRange(
            new AddressType
            {
                Code = "HOME",
                Description = "Home",
                Category = "Shipping",
                AllowsMultiple = true,
                RequiresPrimary = false
            },
            new AddressType
            {
                Code = "WORK",
                Description = "Work",
                Category = "Billing",
                AllowsMultiple = true,
                RequiresPrimary = false
            });

        await db.SaveChangesAsync();
    }

    public static async Task SeedIdentificationTypesAsync(CrmDbContext db)
    {
        if (await db.IdentificationTypes.AnyAsync())
        {
            return;
        }

        db.IdentificationTypes.AddRange(
            new IdentificationType
            {
                Code = "DNI",
                Description = "Documento"
            },
            new IdentificationType
            {
                Code = "PASS",
                Description = "Passport"
            });

        await db.SaveChangesAsync();
    }

    public static async Task SeedStatusesAsync(CrmDbContext db)
    {
        if (await db.Statuses.AnyAsync())
        {
            return;
        }

        db.Statuses.AddRange(
            new Status
            {
                Description = "Active"
            },
            new Status
            {
                Description = "Inactive"
            });

        await db.SaveChangesAsync();
    }

    public static async Task SeedOpportunityStagesAsync(CrmDbContext db)
    {
        if (await db.OpportunityStages.AnyAsync())
        {
            return;
        }

        db.OpportunityStages.AddRange(
            new Domain.Entities.CRM.OpportunityStage
            {
                TenantId = 0,
                Name = "Prospección",
                Order = 1,
                DefaultProbability = 0.1m,
                IsClosed = false,
                IsWon = false
            },
            new Domain.Entities.CRM.OpportunityStage
            {
                TenantId = 0,
                Name = "Calificado",
                Order = 2,
                DefaultProbability = 0.3m,
                IsClosed = false,
                IsWon = false
            },
            new Domain.Entities.CRM.OpportunityStage
            {
                TenantId = 0,
                Name = "Propuesta",
                Order = 3,
                DefaultProbability = 0.5m,
                IsClosed = false,
                IsWon = false
            },
            new Domain.Entities.CRM.OpportunityStage
            {
                TenantId = 0,
                Name = "Negociación",
                Order = 4,
                DefaultProbability = 0.7m,
                IsClosed = false,
                IsWon = false
            },
            new Domain.Entities.CRM.OpportunityStage
            {
                TenantId = 0,
                Name = "Cerrado Ganado",
                Order = 5,
                DefaultProbability = 1.0m,
                IsClosed = true,
                IsWon = true
            },
            new Domain.Entities.CRM.OpportunityStage
            {
                TenantId = 0,
                Name = "Cerrado Perdido",
                Order = 6,
                DefaultProbability = 0.0m,
                IsClosed = true,
                IsWon = false
            });

        await db.SaveChangesAsync();
    }

    public static async Task SeedTenantConfigurationsAsync(CrmDbContext db)
    {
        if (await db.TenantConfigurations.AnyAsync())
        {
            return;
        }

        db.TenantConfigurations.AddRange(
            new Domain.Entities.CRM.TenantConfiguration
            {
                TenantId = 1,
                Key = "limits.maxUsers",
                ValueJson = "{\"value\":25}",
                UpdatedAt = DateTime.UtcNow,
                UpdatedBy = 1
            },
            new Domain.Entities.CRM.TenantConfiguration
            {
                TenantId = 1,
                Key = "features.beta",
                ValueJson = "{\"enabled\":false}",
                UpdatedAt = DateTime.UtcNow,
                UpdatedBy = 1
            });

        await db.SaveChangesAsync();
    }

    public static async Task SeedTenantQuotasAsync(CrmDbContext db)
    {
        if (await db.TenantQuotas.AnyAsync())
        {
            return;
        }

        db.TenantQuotas.Add(
            new Domain.Entities.CRM.TenantQuota
            {
                TenantId = 1,
                Metric = "users",
                Limit = 100,
                Unit = "count",
                EffectiveFrom = DateTime.UtcNow.AddDays(-1)
            });

        await db.SaveChangesAsync();
    }

    public static async Task SeedTenantUsageMetricsAsync(CrmDbContext db)
    {
        if (await db.TenantUsageMetrics.AnyAsync())
        {
            return;
        }

        db.TenantUsageMetrics.Add(
            new Domain.Entities.CRM.TenantUsageMetric
            {
                TenantId = 1,
                Metric = "api.calls",
                Value = 250,
                Unit = "count",
                PeriodStart = DateTime.UtcNow.AddDays(-7),
                PeriodEnd = DateTime.UtcNow,
                Source = "gateway"
            });

        await db.SaveChangesAsync();
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



