using EBOS.CRM.Domain.Entities.EBOS;
using EBOS.CRM.Infrastructure.Persistence;
using EBOS.CRM.Infrastructure.Repositories.Concrete.EBOS;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EBOS.CRM.ApiTests.Infrastructure.Repositories.EBOS;

public class EbosReadOnlyRepositoryContractTest
{
    [Fact]
    public async Task AddressTypeRepository_CommonReadContract_Works()
    {
        var options = BuildOptions();
        await using var context = new CrmDbContext(options);
        context.AddressTypes.AddRange(
            new AddressType { Code = "HOME", Description = "Home", Category = "Shipping", CreatedAt = DateTime.UtcNow, CreatedBy = 1 },
            new AddressType { Code = "WORK", Description = "Work", Category = "Billing", CreatedAt = DateTime.UtcNow, CreatedBy = 1 });
        await context.SaveChangesAsync();

        var repo = new AddressTypeRepository(context);
        Assert.Equal(2, await repo.CountAsync());
        Assert.Equal(2, (await repo.GetAllAsync()).Count);
        Assert.Single(await repo.GetAllPagedAsync(1, 1));
        Assert.Equal(2, repo.AsQueryable().Count());
        Assert.NotNull(await repo.GetByIdAsync(context.AddressTypes.First().Id));
    }

    [Fact]
    public async Task CountryRepository_CommonReadContract_Works()
    {
        var options = BuildOptions();
        await using var context = new CrmDbContext(options);
        context.Countries.AddRange(
            NewCountry("ES", "ESP", "724", "Spain"),
            NewCountry("FR", "FRA", "250", "France"));
        await context.SaveChangesAsync();

        var repo = new CountryRepository(context);
        Assert.Equal(2, await repo.CountAsync());
        Assert.Equal(2, (await repo.GetAllAsync()).Count);
        Assert.Single(await repo.GetAllPagedAsync(1, 1));
        Assert.Equal(2, repo.AsQueryable().Count());
    }

    [Fact]
    public async Task ChannelTypeRepository_CommonReadContract_Works()
    {
        var options = BuildOptions();
        await using var context = new CrmDbContext(options);
        context.ChannelTypes.AddRange(
            new ChannelType { Descripcion = "EMAIL", IsActive = true, CreatedAt = DateTime.UtcNow, CreatedBy = 1, UpdatedAt = DateTime.UtcNow, UpdatedBy = 1 },
            new ChannelType { Descripcion = "SMS", IsActive = true, CreatedAt = DateTime.UtcNow, CreatedBy = 1, UpdatedAt = DateTime.UtcNow, UpdatedBy = 1 });
        await context.SaveChangesAsync();

        var repo = new ChannelTypeRepository(context);
        Assert.Equal(2, await repo.CountAsync());
        Assert.Equal(2, (await repo.GetAllAsync()).Count);
        Assert.Single(await repo.GetAllPagedAsync(1, 1));
    }

    [Fact]
    public async Task IdentificationTypeRepository_CommonReadContract_Works()
    {
        var options = BuildOptions();
        await using var context = new CrmDbContext(options);
        context.IdentificationTypes.AddRange(
            new IdentificationType { Code = "DNI", Description = "Documento", CreatedAt = DateTime.UtcNow, CreatedBy = 1 },
            new IdentificationType { Code = "PASS", Description = "Passport", CreatedAt = DateTime.UtcNow, CreatedBy = 1 });
        await context.SaveChangesAsync();

        var repo = new IdentificationTypeRepository(context);
        Assert.Equal(2, await repo.CountAsync());
        Assert.Equal(2, (await repo.GetAllAsync()).Count);
        Assert.Single(await repo.GetAllPagedAsync(1, 1));
    }

    [Fact]
    public async Task StatusRepository_CommonReadContract_Works()
    {
        var options = BuildOptions();
        await using var context = new CrmDbContext(options);
        context.Statuses.AddRange(
            new Status { Description = "Active", CreatedAt = DateTime.UtcNow, CreatedBy = 1 },
            new Status { Description = "Inactive", CreatedAt = DateTime.UtcNow, CreatedBy = 1 });
        await context.SaveChangesAsync();

        var repo = new StatusRepository(context);
        Assert.Equal(2, await repo.CountAsync());
        Assert.Equal(2, (await repo.GetAllAsync()).Count);
        Assert.Single(await repo.GetAllPagedAsync(1, 1));
    }

    [Fact]
    public async Task ChannelCountryRepository_ReadContract_AndIsAllowed_Works()
    {
        var options = BuildOptions();
        await using var context = new CrmDbContext(options);
        var country = NewCountry("ES", "ESP", "724", "Spain");
        var channel = new ChannelType { Descripcion = "EMAIL", IsActive = true, CreatedAt = DateTime.UtcNow, CreatedBy = 1, UpdatedAt = DateTime.UtcNow, UpdatedBy = 1 };
        context.Countries.Add(country);
        context.ChannelTypes.Add(channel);
        await context.SaveChangesAsync();
        context.ChannelCountries.AddRange(
            new ChannelCountry { CountryId = country.Id, ChannelTypeId = channel.Id, IsActive = true, CreatedAt = DateTime.UtcNow, CreatedBy = 1, UpdatedAt = DateTime.UtcNow, UpdatedBy = 1 },
            new ChannelCountry { CountryId = country.Id, ChannelTypeId = channel.Id, IsActive = false, CreatedAt = DateTime.UtcNow, CreatedBy = 1, UpdatedAt = DateTime.UtcNow, UpdatedBy = 1 });
        await context.SaveChangesAsync();

        var repo = new ChannelCountryRepository(context);
        Assert.Equal(2, await repo.CountAsync());
        Assert.Equal(2, (await repo.GetAllAsync()).Count);
        Assert.Single(await repo.GetAllPagedAsync(1, 1));
        Assert.True(await repo.IsAllowedAsync(channel.Id, country.Id, CancellationToken.None));
    }

    [Fact]
    public async Task ValidationRuleRepository_ReadContract_AndGetByKeys_Works()
    {
        var options = BuildOptions();
        await using var context = new CrmDbContext(options);
        context.ValidationRules.AddRange(
            new ValidationRule { Key = "postal_code:ES", Pattern = "^[0-9]{5}$", IsActive = true, CreatedAt = DateTime.UtcNow, CreatedBy = 1, Erased = false },
            new ValidationRule { Key = "phone:ES", Pattern = "^[0-9]{9}$", IsActive = false, CreatedAt = DateTime.UtcNow, CreatedBy = 1, Erased = false },
            new ValidationRule { Key = "tax_id:ES", Pattern = "^[A-Z0-9]+$", IsActive = true, CreatedAt = DateTime.UtcNow, CreatedBy = 1, Erased = true });
        await context.SaveChangesAsync();

        var repo = new ValidationRuleRepository(context);
        Assert.Equal(2, await repo.CountAsync());
        Assert.Equal(2, (await repo.GetAllAsync()).Count);
        Assert.Single(await repo.GetAllPagedAsync(1, 1));
        Assert.Equal(2, repo.AsQueryable().Count()); // excludes erased

        var byKeys = await repo.GetByKeysAsync(new[] { "postal_code:ES", "phone:ES", "tax_id:ES" });
        Assert.Single(byKeys); // only active + not erased
        Assert.Equal("postal_code:ES", byKeys.First().Key);
    }

    private static DbContextOptions<CrmDbContext> BuildOptions()
    {
        var serviceProvider = new ServiceCollection()
            .AddEntityFrameworkInMemoryDatabase()
            .BuildServiceProvider();

        return new DbContextOptionsBuilder<CrmDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
            .UseInternalServiceProvider(serviceProvider)
            .Options;
    }

    private static Country NewCountry(string a2, string a3, string num, string name)
        => new()
        {
            Name = name,
            Iso31661A2Code = a2,
            Iso31661A3Code = a3,
            Iso31661NumCode = num,
            Domain = a2.ToLowerInvariant(),
            Currency = "Euro",
            CurrencyCode = "EUR",
            InternationalPhoneCode = "+34",
            CreatedAt = DateTime.UtcNow,
            CreatedBy = 1
        };
}
