using EBOS.CRM.Application.Services.Interfaces;
using EBOS.CRM.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EBOS.CRM.Infrastructure.Services.Lookup;

public sealed class LookupNormalizationService(CrmDbContext db) : ILookupNormalizationService
{
    public async Task NormalizeAsync(CancellationToken cancellationToken = default)
    {
        await NormalizeCountriesAsync(cancellationToken);
        await NormalizeAddressTypesAsync(cancellationToken);
        await NormalizeIdentificationTypesAsync(cancellationToken);
        await NormalizeStatusesAsync(cancellationToken);
    }

    private async Task NormalizeCountriesAsync(CancellationToken cancellationToken)
    {
        var countries = await db.Countries.OrderBy(c => c.Id).ToListAsync(cancellationToken);
        var duplicateGroups = countries.GroupBy(c => c.Iso31661A2Code)
            .Where(g => g.Count() > 1)
            .ToList();

        foreach (var group in duplicateGroups)
        {
            var keep = group.First();
            var remove = group.Skip(1).ToList();
            var removeIds = remove.Select(c => c.Id).ToList();

            var addresses = await db.Addresses.Where(a => removeIds.Contains(a.CountryId))
                .ToListAsync(cancellationToken);
            foreach (var address in addresses)
            {
                address.CountryId = keep.Id;
            }

            db.Countries.RemoveRange(remove);
        }

        await db.SaveChangesAsync(cancellationToken);
    }

    private async Task NormalizeAddressTypesAsync(CancellationToken cancellationToken)
    {
        var addressTypes = await db.AddressTypes.OrderBy(a => a.Id).ToListAsync(cancellationToken);
        var duplicateGroups = addressTypes.GroupBy(a => a.Code)
            .Where(g => g.Count() > 1)
            .ToList();

        foreach (var group in duplicateGroups)
        {
            var keep = group.First();
            var remove = group.Skip(1).ToList();
            var removeIds = remove.Select(a => a.Id).ToList();

            var addresses = await db.Addresses.Where(a => removeIds.Contains(a.AddressTypeId))
                .ToListAsync(cancellationToken);
            foreach (var address in addresses)
            {
                address.AddressTypeId = keep.Id;
            }

            db.AddressTypes.RemoveRange(remove);
        }

        await db.SaveChangesAsync(cancellationToken);
    }

    private async Task NormalizeIdentificationTypesAsync(CancellationToken cancellationToken)
    {
        var identificationTypes = await db.IdentificationTypes.OrderBy(i => i.Id).ToListAsync(cancellationToken);
        var duplicateGroups = identificationTypes.GroupBy(i => i.Code)
            .Where(g => g.Count() > 1)
            .ToList();

        foreach (var group in duplicateGroups)
        {
            var keep = group.First();
            var remove = group.Skip(1).ToList();
            var removeIds = remove.Select(i => i.Id).ToList();

            var customers = await db.IndividualCustomers.Where(c => removeIds.Contains(c.IdentificationTypeId))
                .ToListAsync(cancellationToken);
            foreach (var customer in customers)
            {
                customer.IdentificationTypeId = keep.Id;
            }

            db.IdentificationTypes.RemoveRange(remove);
        }

        await db.SaveChangesAsync(cancellationToken);
    }

    private async Task NormalizeStatusesAsync(CancellationToken cancellationToken)
    {
        var statuses = await db.Statuses.OrderBy(s => s.Id).ToListAsync(cancellationToken);
        var duplicateGroups = statuses.GroupBy(s => s.Description)
            .Where(g => g.Count() > 1)
            .ToList();

        foreach (var group in duplicateGroups)
        {
            var keep = group.First();
            var remove = group.Skip(1).ToList();
            var removeIds = remove.Select(s => s.Id).ToList();

            var customers = await db.Customers.Where(c => removeIds.Contains(c.StatusId))
                .ToListAsync(cancellationToken);
            foreach (var customer in customers)
            {
                customer.StatusId = keep.Id;
            }

            db.Statuses.RemoveRange(remove);
        }

        await db.SaveChangesAsync(cancellationToken);
    }
}
