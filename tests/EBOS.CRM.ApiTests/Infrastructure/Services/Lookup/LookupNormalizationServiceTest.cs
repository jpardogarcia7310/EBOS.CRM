using EBOS.CRM.Domain.Entities.EBOS;
using EBOS.CRM.Infrastructure.Persistence;
using EBOS.CRM.Infrastructure.Services.Lookup;
using Microsoft.EntityFrameworkCore;

namespace EBOS.CRM.ApiTests.Infrastructure.Services.Lookup;

public class LookupNormalizationServiceTest
{
    [Fact]
    public async Task NormalizeAsync_RemovesDuplicateCountryAndRepointsAddresses()
    {
        var options = new DbContextOptionsBuilder<CrmDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
            .Options;
        await using var db = new CrmDbContext(options);

        var c1 = new Country { Id = 1, Name = "A", Iso31661A2Code = "ES", Iso31661A3Code = "ESP", Iso31661NumCode = "724", Domain = ".es", Currency = "EUR", CurrencyCode = "EUR", InternationalPhoneCode = "+34" };
        var c2 = new Country { Id = 2, Name = "B", Iso31661A2Code = "ES", Iso31661A3Code = "ESP", Iso31661NumCode = "724", Domain = ".es", Currency = "EUR", CurrencyCode = "EUR", InternationalPhoneCode = "+34" };
        db.Countries.AddRange(c1, c2);
        db.AddressTypes.Add(new AddressType { Id = 1, Code = "HOME", Description = "H", Category = "C", AllowsMultiple = true, RequiresPrimary = false });
        db.Statuses.Add(new Status { Id = 1, Description = "Active" });
        db.Addresses.Add(new global::EBOS.CRM.Domain.Entities.CRM.Address { Id = 10, TenantId = 1, Street = "s", ExternalNumber = "1", City = "x", StateOrProvince = "y", PostalCode = "z", CountryId = 2, AddressTypeId = 1 });
        await db.SaveChangesAsync();

        var sut = new LookupNormalizationService(db);
        await sut.NormalizeAsync(CancellationToken.None);

        var countries = await db.Countries.ToListAsync();
        var addr = await db.Addresses.SingleAsync();
        Assert.Single(countries, x => x.Iso31661A2Code == "ES");
        Assert.Equal(1, addr.CountryId);
    }
}
