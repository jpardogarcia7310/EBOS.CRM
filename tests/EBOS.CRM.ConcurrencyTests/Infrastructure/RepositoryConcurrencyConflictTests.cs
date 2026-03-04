using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Entities.EBOS;
using EBOS.CRM.Infrastructure.Persistence;
using EBOS.CRM.Infrastructure.Repositories.Concrete.CRM;
using Microsoft.EntityFrameworkCore;

namespace EBOS.CRM.ConcurrencyTests.Infrastructure;

public class RepositoryConcurrencyConflictTests
{
    [Fact]
    public async Task CustomerPreferenceRepository_WhenConcurrentUpdates_ThrowsDbUpdateConcurrencyException()
    {
        var dbName = Guid.NewGuid().ToString("N");
        await using var seedContext = CreateContext(dbName);

        var (customerId, channelId) = await SeedCustomerAndChannelAsync(seedContext);
        var preference = CustomerPreference.Create(1, customerId, channelId, true, DateTime.UtcNow.AddDays(-1), 1);
        seedContext.CustomerPreferences.Add(preference);
        await seedContext.SaveChangesAsync();

        await using var contextA = CreateContext(dbName);
        await using var contextB = CreateContext(dbName);
        var repoA = new CustomerPreferenceRepository(contextA);
        var repoB = new CustomerPreferenceRepository(contextB);

        var entityA = await repoA.GetByIdAsync(preference.Id);
        var entityB = await repoB.GetByIdAsync(preference.Id);
        Assert.NotNull(entityA);
        Assert.NotNull(entityB);

        entityA.UpdatePreference(false, DateTime.UtcNow, 10);
        contextA.Entry(entityA!).Property("RowVersion").CurrentValue = new byte[] { 2 };
        await repoA.UpdateAsync(entityA);
        await repoA.SaveChangesAsync();

        entityB!.UpdatePreference(true, DateTime.UtcNow, 11);
        await repoB.UpdateAsync(entityB);

        await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => repoB.SaveChangesAsync());
    }

    [Fact]
    public async Task CustomerConsentRepository_WhenConcurrentUpdates_ThrowsDbUpdateConcurrencyException()
    {
        var dbName = Guid.NewGuid().ToString("N");
        await using var seedContext = CreateContext(dbName);

        var (customerId, _) = await SeedCustomerAndChannelAsync(seedContext);
        var consent = CustomerConsent.Create(1, customerId, "MARKETING_EMAIL", true, DateTime.UtcNow, "it", null);
        seedContext.CustomerConsents.Add(consent);
        await seedContext.SaveChangesAsync();

        await using var contextA = CreateContext(dbName);
        await using var contextB = CreateContext(dbName);
        var repoA = new CustomerConsentRepository(contextA);
        var repoB = new CustomerConsentRepository(contextB);

        var entityA = await repoA.GetByIdAsync(consent.Id);
        var entityB = await repoB.GetByIdAsync(consent.Id);
        Assert.NotNull(entityA);
        Assert.NotNull(entityB);

        contextA.Entry(entityA!).Property("RowVersion").CurrentValue = new byte[] { 6 };
        await repoA.UpdateAsync(entityA);
        await repoA.SaveChangesAsync();

        contextB.Entry(entityB!).Property("RowVersion").CurrentValue = Array.Empty<byte>();
        await repoB.UpdateAsync(entityB);

        await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => repoB.SaveChangesAsync());
    }

    private static CrmDbContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<CrmDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        return new CrmDbContext(options);
    }

    private static async Task<(long customerId, long channelId)> SeedCustomerAndChannelAsync(CrmDbContext context)
    {
        var status = new Status
        {
            Description = "Active",
            CreatedAt = DateTime.UtcNow,
            CreatedBy = 1
        };

        var channel = new ChannelType
        {
            Descripcion = "Email",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedBy = 1,
            UpdatedBy = 1
        };

        context.Statuses.Add(status);
        context.ChannelTypes.Add(channel);
        await context.SaveChangesAsync();

        var customer = new Customer
        {
            TenantId = 1,
            Code = $"C-{Guid.NewGuid():N}"[..12],
            Email = $"c-{Guid.NewGuid():N}@example.com",
            Phone = "34600000009",
            StatusId = status.Id,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = 1
        };

        context.Customers.Add(customer);
        await context.SaveChangesAsync();
        return (customer.Id, channel.Id);
    }
}
