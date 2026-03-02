using EBOS.CRM.ConcurrencyTests.Fixtures;
using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services.CRM;
using EBOS.CRM.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using CRMCorporateCustomer = EBOS.CRM.Domain.Entities.CRM.CorporateCustomer;
using CRMCustomer = EBOS.CRM.Domain.Entities.CRM.Customer;

namespace EBOS.CRM.ConcurrencyTests.Controllers.CRM.Customer360;

public class Customer360ConcurrencyTests(ConcurrencyWebApplicationFactory<Program> factory)
    : IClassFixture<ConcurrencyWebApplicationFactory<Program>>
{
    [Fact]
    public async Task CustomerConsentRepository_ConcurrentReads_DoNotFail()
    {
        var customerId = await SeedCustomerWithConsentsAsync(factory.Services);

        var tasks = Enumerable.Range(0, 50).Select(async _ =>
        {
            using var scope = factory.Services.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<ICustomerConsentRepository>();
            return await repo.CountLatestByCustomerAsync(1, customerId);
        });

        var counts = await Task.WhenAll(tasks);

        Assert.All(counts, c => Assert.True(c >= 1));
    }

    [Fact]
    public async Task AccountHierarchyCycleGuard_ConcurrentChecks_AreStable()
    {
        var (aId, cId) = await SeedHierarchyForCycleAsync(factory.Services);

        var tasks = Enumerable.Range(0, 40).Select(async _ =>
        {
            using var scope = factory.Services.CreateScope();
            var guard = scope.ServiceProvider.GetRequiredService<IAccountHierarchyCycleGuard>();
            return await guard.CreatesCycleAsync(1, cId, aId);
        });

        var results = await Task.WhenAll(tasks);

        Assert.All(results, result => Assert.True(result));
    }

    private static async Task<long> SeedCustomerWithConsentsAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CrmDbContext>();
        var statusId = db.Statuses.Select(x => x.Id).First();

        var customer = new CRMCustomer
        {
            TenantId = 1,
            Code = $"C360-{Guid.NewGuid():N}"[..20],
            Email = $"c360-{Guid.NewGuid():N}@example.com",
            Phone = "123456789012",
            StatusId = statusId,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = 1
        };

        db.Customers.Add(customer);
        await db.SaveChangesAsync();

        var revokedAt = DateTime.UtcNow.AddDays(-1);
        db.CustomerConsents.AddRange(
            CustomerConsent.Create(1, customer.Id, "MARKETING_EMAIL", true, DateTime.UtcNow.AddDays(-10), "seed", null),
            CustomerConsent.CreateRevoked(1, customer.Id, "MARKETING_EMAIL", revokedAt, "seed", revokedAt));
        await db.SaveChangesAsync();

        return customer.Id;
    }

    private static async Task<(long aId, long cId)> SeedHierarchyForCycleAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CrmDbContext>();
        var statusId = db.Statuses.Select(x => x.Id).First();

        var a = new CRMCorporateCustomer
        {
            TenantId = 1,
            Code = $"A-{Guid.NewGuid():N}"[..20],
            Email = $"a-{Guid.NewGuid():N}@example.com",
            Phone = "123456789012",
            StatusId = statusId,
            LegalName = "A Corp",
            TaxIdentification = $"A{Random.Shared.Next(100000, 999999)}",
            CreatedAt = DateTime.UtcNow,
            CreatedBy = 1
        };
        var b = new CRMCorporateCustomer
        {
            TenantId = 1,
            Code = $"B-{Guid.NewGuid():N}"[..20],
            Email = $"b-{Guid.NewGuid():N}@example.com",
            Phone = "123456789012",
            StatusId = statusId,
            LegalName = "B Corp",
            TaxIdentification = $"B{Random.Shared.Next(100000, 999999)}",
            CreatedAt = DateTime.UtcNow,
            CreatedBy = 1
        };
        var c = new CRMCorporateCustomer
        {
            TenantId = 1,
            Code = $"C-{Guid.NewGuid():N}"[..20],
            Email = $"c-{Guid.NewGuid():N}@example.com",
            Phone = "123456789012",
            StatusId = statusId,
            LegalName = "C Corp",
            TaxIdentification = $"C{Random.Shared.Next(100000, 999999)}",
            CreatedAt = DateTime.UtcNow,
            CreatedBy = 1
        };

        db.CorporateCustomers.AddRange(a, b, c);
        await db.SaveChangesAsync();

        db.AccountHierarchies.AddRange(
            AccountHierarchy.Create(1, a.Id, b.Id, "HOLDING", DateTime.UtcNow.AddDays(-5)),
            AccountHierarchy.Create(1, b.Id, c.Id, "HOLDING", DateTime.UtcNow.AddDays(-4)));
        await db.SaveChangesAsync();

        return (a.Id, c.Id);
    }
}
