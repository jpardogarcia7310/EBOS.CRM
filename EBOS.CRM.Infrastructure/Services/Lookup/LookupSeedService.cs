using EBOS.CRM.Application.Services.Interfaces;
using EBOS.CRM.Domain.Entities;
using EBOS.CRM.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EBOS.CRM.Infrastructure.Services.Lookup;

public sealed class LookupSeedService(CrmDbContext db, ILookupNormalizationService normalizationService)
    : ILookupSeedService
{
    public async Task EnsureCanonicalLookupsAsync(CancellationToken cancellationToken = default)
    {
        await normalizationService.NormalizeAsync(cancellationToken);

        await EnsureCountriesAsync(cancellationToken);
        await EnsureAddressTypesAsync(cancellationToken);
        await EnsureIdentificationTypesAsync(cancellationToken);
        await EnsureStatusesAsync(cancellationToken);

        await normalizationService.NormalizeAsync(cancellationToken);
    }

    private async Task EnsureCountriesAsync(CancellationToken cancellationToken)
    {
        var canonical = new[]
        {
            new Country
            {
                Name = "España",
                Iso31661A2Code = "ES",
                Iso31661A3Code = "ESP",
                Iso31661NumCode = "724",
                Domain = "es",
                Currency = "Euro",
                CurrencyCode = "EUR",
                InternationalPhoneCode = "+34"
            },
            new Country
            {
                Name = "Francia",
                Iso31661A2Code = "FR",
                Iso31661A3Code = "FRA",
                Iso31661NumCode = "250",
                Domain = "fr",
                Currency = "Euro",
                CurrencyCode = "EUR",
                InternationalPhoneCode = "+33"
            },
            new Country
            {
                Name = "Alemania",
                Iso31661A2Code = "DE",
                Iso31661A3Code = "DEU",
                Iso31661NumCode = "276",
                Domain = "de",
                Currency = "Euro",
                CurrencyCode = "EUR",
                InternationalPhoneCode = "+49"
            },
            new Country
            {
                Name = "Italia",
                Iso31661A2Code = "IT",
                Iso31661A3Code = "ITA",
                Iso31661NumCode = "380",
                Domain = "it",
                Currency = "Euro",
                CurrencyCode = "EUR",
                InternationalPhoneCode = "+39"
            }
        };

        foreach (var entry in canonical)
        {
            var existing = await db.Countries.FirstOrDefaultAsync(c => c.Iso31661A2Code == entry.Iso31661A2Code, cancellationToken);
            if (existing == null)
            {
                db.Countries.Add(entry);
                continue;
            }

            existing.Name = entry.Name;
            existing.Iso31661A3Code = entry.Iso31661A3Code;
            existing.Iso31661NumCode = entry.Iso31661NumCode;
            existing.Domain = entry.Domain;
            existing.Currency = entry.Currency;
            existing.CurrencyCode = entry.CurrencyCode;
            existing.InternationalPhoneCode = entry.InternationalPhoneCode;
        }

        await db.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureAddressTypesAsync(CancellationToken cancellationToken)
    {
        var canonical = new[]
        {
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
            }
        };

        foreach (var entry in canonical)
        {
            var existing = await db.AddressTypes.FirstOrDefaultAsync(a => a.Code == entry.Code, cancellationToken);
            if (existing == null)
            {
                db.AddressTypes.Add(entry);
                continue;
            }

            existing.Description = entry.Description;
            existing.Category = entry.Category;
            existing.AllowsMultiple = entry.AllowsMultiple;
            existing.RequiresPrimary = entry.RequiresPrimary;
        }

        await db.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureIdentificationTypesAsync(CancellationToken cancellationToken)
    {
        var canonical = new[]
        {
            new IdentificationType
            {
                Code = "DNI",
                Description = "Documento"
            },
            new IdentificationType
            {
                Code = "PASS",
                Description = "Passport"
            }
        };

        foreach (var entry in canonical)
        {
            var existing = await db.IdentificationTypes.FirstOrDefaultAsync(i => i.Code == entry.Code, cancellationToken);
            if (existing == null)
            {
                db.IdentificationTypes.Add(entry);
                continue;
            }

            existing.Description = entry.Description;
        }

        await db.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureStatusesAsync(CancellationToken cancellationToken)
    {
        var canonical = new[]
        {
            new Status { Description = "Active" },
            new Status { Description = "Inactive" }
        };

        foreach (var entry in canonical)
        {
            var exists = await db.Statuses.AnyAsync(s => s.Description == entry.Description, cancellationToken);
            if (!exists)
            {
                db.Statuses.Add(entry);
            }
        }

        await db.SaveChangesAsync(cancellationToken);
    }
}

