using EBOS.CRM.Application.Services.Interfaces;
using EBOS.CRM.Domain.Entities.CRM;
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
        var context = new CrmDbContext(options, new TestCurrentUserContext(1));

        context.Addresses.AddRange(
            BuildAddress(tenantId: 1, street: "One St"),
            BuildAddress(tenantId: 2, street: "Two St")
        );
        await context.SaveChangesAsync();

        var result = await context.Addresses.ToListAsync();

        Assert.Single(result);
        Assert.Equal(1, result[0].TenantId);
    }

    [Fact]
    public async Task QueryFilter_DoesNotFilter_WhenTenantMissing()
    {
        var options = BuildOptions();
        var context = new CrmDbContext(options, new TestCurrentUserContext(0));

        context.Addresses.AddRange(
            BuildAddress(tenantId: 1, street: "One St"),
            BuildAddress(tenantId: 2, street: "Two St")
        );
        await context.SaveChangesAsync();

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
