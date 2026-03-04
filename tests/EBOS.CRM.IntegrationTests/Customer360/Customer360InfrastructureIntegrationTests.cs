using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services.CRM;
using EBOS.CRM.Infrastructure.Persistence;
using EBOS.CRM.IntegrationTests.Infrastructure;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace EBOS.CRM.IntegrationTests.Customer360;

public class Customer360InfrastructureIntegrationTests(CustomWebApplicationFactory factory)
    : IClassFixture<CustomWebApplicationFactory>
{
    [Fact]
    public async Task AccountHierarchyCycleGuard_DetectsMultiHopCycle()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CrmDbContext>();
        var guard = scope.ServiceProvider.GetRequiredService<IAccountHierarchyCycleGuard>();

        var statusId = db.Statuses.Select(x => x.Id).First();
        var a = CreateCorporate(1, statusId, "A");
        var b = CreateCorporate(1, statusId, "B");
        var c = CreateCorporate(1, statusId, "C");
        db.CorporateCustomers.AddRange(a, b, c);
        await db.SaveChangesAsync();

        db.AccountHierarchies.AddRange(
            AccountHierarchy.Create(1, a.Id, b.Id, "HOLDING", DateTime.UtcNow.AddDays(-10)),
            AccountHierarchy.Create(1, b.Id, c.Id, "HOLDING", DateTime.UtcNow.AddDays(-9)));
        await db.SaveChangesAsync();

        var createsCycle = await guard.CreatesCycleAsync(1, c.Id, a.Id);

        createsCycle.Should().BeTrue();
    }

    [Fact]
    public async Task CustomerConsentRepository_GetLatestByCustomerPaged_ReturnsLatestByConsentType()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CrmDbContext>();
        var repository = scope.ServiceProvider.GetRequiredService<ICustomerConsentRepository>();

        var statusId = db.Statuses.Select(x => x.Id).First();
        var customer = CreateBaseCustomer(1, statusId, "CONS");
        db.Customers.Add(customer);
        await db.SaveChangesAsync();

        var revokedAt = DateTime.UtcNow.AddDays(-1);
        db.CustomerConsents.AddRange(
            CustomerConsent.Create(1, customer.Id, "MARKETING_EMAIL", true, DateTime.UtcNow.AddDays(-5), "web", null),
            CustomerConsent.CreateRevoked(1, customer.Id, "MARKETING_EMAIL", revokedAt, "web", revokedAt),
            CustomerConsent.Create(1, customer.Id, "PRODUCT_UPDATES_SMS", true, DateTime.UtcNow.AddDays(-2), "call-center", null));
        await db.SaveChangesAsync();

        var latest = await repository.GetLatestByCustomerPagedAsync(1, customer.Id, 1, 10);

        latest.Should().HaveCount(2);
        latest.Should().Contain(x => x.ConsentType == "MARKETING_EMAIL" && !x.Granted);
        latest.Should().Contain(x => x.ConsentType == "PRODUCT_UPDATES_SMS" && x.Granted);
    }

    private static CorporateCustomer CreateCorporate(long tenantId, long statusId, string suffix) => new()
    {
        TenantId = tenantId,
        Code = $"CORP-{suffix}-{Guid.NewGuid():N}"[..20],
        Email = $"corp-{suffix}-{Guid.NewGuid():N}@example.com",
        Phone = "123456789012",
        StatusId = statusId,
        LegalName = $"Corp {suffix}",
        TaxIdentification = $"TAX{Random.Shared.Next(100000, 999999)}",
        CreatedAt = DateTime.UtcNow,
        CreatedBy = 1
    };

    private static Customer CreateBaseCustomer(long tenantId, long statusId, string suffix) => new()
    {
        TenantId = tenantId,
        Code = $"CUS-{suffix}-{Guid.NewGuid():N}"[..20],
        Email = $"cus-{suffix}-{Guid.NewGuid():N}@example.com",
        Phone = "123456789012",
        StatusId = statusId,
        CreatedAt = DateTime.UtcNow,
        CreatedBy = 1
    };
}
