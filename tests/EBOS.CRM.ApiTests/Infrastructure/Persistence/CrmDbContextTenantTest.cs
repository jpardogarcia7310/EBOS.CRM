using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Interfaces.Services;
using EBOS.CRM.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EBOS.CRM.ApiTests.Infrastructure.Persistence;

public class CrmDbContextTenantTest
{
    [Fact]
    public async Task QueryFilter_FiltersByTenant_WhenTenantProvided()
    {
        var options = BuildOptions();
        var seedContext = new CrmDbContext(options, new TestCurrentUserContext(0));
        seedContext.Addresses.AddRange(
            BuildAddress(tenantId: 1, street: "One St"),
            BuildAddress(tenantId: 2, street: "Two St")
        );
        await seedContext.SaveChangesAsync();

        var context = new CrmDbContext(options, new TestCurrentUserContext(1));
        var result = await context.Addresses.ToListAsync();

        Assert.Single(result);
        Assert.Equal(1, result[0].TenantId);
    }

    [Fact]
    public async Task QueryFilter_DoesNotFilter_WhenTenantMissing()
    {
        var options = BuildOptions();
        var seedContext = new CrmDbContext(options, new TestCurrentUserContext(0));
        seedContext.Addresses.AddRange(
            BuildAddress(tenantId: 1, street: "One St"),
            BuildAddress(tenantId: 2, street: "Two St")
        );
        await seedContext.SaveChangesAsync();

        var context = new CrmDbContext(options, new TestCurrentUserContext(0));
        var result = await context.Addresses.ToListAsync();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task SaveChanges_AssignsTenantId_WhenMissing()
    {
        var options = BuildOptions();
        var context = new CrmDbContext(options, new TestCurrentUserContext(7));

        var address = BuildAddress(tenantId: 0, street: "Auto");
        context.Addresses.Add(address);

        await context.SaveChangesAsync();

        Assert.Equal(7, address.TenantId);
    }

    [Fact]
    public async Task SaveChanges_Throws_WhenTenantMismatch_OnAdd()
    {
        var options = BuildOptions();
        var context = new CrmDbContext(options, new TestCurrentUserContext(7));

        var address = BuildAddress(tenantId: 9, street: "Mismatch");
        context.Addresses.Add(address);

        var act = () => context.SaveChangesAsync();

        await Assert.ThrowsAsync<InvalidOperationException>(act);
    }

    [Fact]
    public async Task SaveChanges_Throws_WhenTenantMissing_OnUpdate()
    {
        var options = BuildOptions();
        var context = new CrmDbContext(options, new TestCurrentUserContext(7));

        var address = BuildAddress(tenantId: 7, street: "Update");
        context.Addresses.Add(address);
        await context.SaveChangesAsync();

        address.TenantId = 0;
        address.Street = "Update-2";

        var act = () => context.SaveChangesAsync();

        await Assert.ThrowsAsync<InvalidOperationException>(act);
    }

    [Fact]
    public async Task SaveChanges_Allows_WhenTenantMatches_OnAdd()
    {
        var options = BuildOptions();
        var context = new CrmDbContext(options, new TestCurrentUserContext(7));

        var address = BuildAddress(tenantId: 7, street: "Match");
        context.Addresses.Add(address);

        await context.SaveChangesAsync();

        Assert.Equal(7, address.TenantId);
    }

    [Fact]
    public async Task SaveChanges_Throws_WhenTenantMismatch_OnUpdate()
    {
        var options = BuildOptions();
        var seedContext = new CrmDbContext(options, new TestCurrentUserContext(0));
        var address = BuildAddress(tenantId: 9, street: "MismatchUpdate");
        seedContext.Addresses.Add(address);
        await seedContext.SaveChangesAsync();

        var context = new CrmDbContext(options, new TestCurrentUserContext(7));
        var tracked = await context.Addresses.IgnoreQueryFilters().FirstAsync();
        tracked.Street = "Changed";

        var act = () => context.SaveChangesAsync();

        await Assert.ThrowsAsync<InvalidOperationException>(act);
    }

    [Fact]
    public async Task SaveChanges_Throws_WhenTenantMismatch_OnDelete()
    {
        var options = BuildOptions();
        var seedContext = new CrmDbContext(options, new TestCurrentUserContext(0));
        var address = BuildAddress(tenantId: 9, street: "MismatchDelete");
        seedContext.Addresses.Add(address);
        await seedContext.SaveChangesAsync();

        var context = new CrmDbContext(options, new TestCurrentUserContext(7));
        var tracked = await context.Addresses.IgnoreQueryFilters().FirstAsync();
        context.Addresses.Remove(tracked);

        var act = () => context.SaveChangesAsync();

        await Assert.ThrowsAsync<InvalidOperationException>(act);
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

    private static Address BuildAddress(long tenantId, string street)
        => new()
        {
            TenantId = tenantId,
            Street = street,
            ExternalNumber = "1",
            City = "City",
            StateOrProvince = "State",
            PostalCode = "0000",
            CountryId = 1,
            AddressTypeId = 1
        };

    private sealed class TestCurrentUserContext(long tenantId) : ICurrentUserContext
    {
        public long UserId => 0;
        public long TenantId => tenantId;
        public string CorrelationId => Guid.NewGuid().ToString("D");
    }
}
