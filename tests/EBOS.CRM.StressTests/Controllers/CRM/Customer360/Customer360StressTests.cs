using EBOS.CRM.ApiTests.Fixtures;
using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using CRMCustomer = EBOS.CRM.Domain.Entities.CRM.Customer;

namespace EBOS.CRM.StressTests.Controllers.CRM.Customer360;

public class Customer360StressTests(CustomWebApplicationFactory<Program> factory)
    : IClassFixture<CustomWebApplicationFactory<Program>>
{
    [Fact]
    public async Task CustomerConsentRepository_HighVolumeLatestReads_Work()
    {
        var customerId = await SeedCustomerWithConsentHistoryAsync(factory.Services);

        var tasks = Enumerable.Range(0, 150).Select(async _ =>
        {
            using var scope = factory.Services.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<ICustomerConsentRepository>();
            var latest = await repo.GetLatestByCustomerPagedAsync(1, customerId, 1, 20);
            return latest.Count;
        });

        var counts = await Task.WhenAll(tasks);

        Assert.All(counts, c => Assert.True(c >= 2));
    }

    private static async Task<long> SeedCustomerWithConsentHistoryAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CrmDbContext>();
        var statusId = db.Statuses.Select(x => x.Id).First();

        var customer = new CRMCustomer
        {
            TenantId = 1,
            Code = $"STR-{Guid.NewGuid():N}"[..20],
            Email = $"str-{Guid.NewGuid():N}@example.com",
            Phone = "123456789012",
            StatusId = statusId,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = 1
        };

        db.Customers.Add(customer);
        await db.SaveChangesAsync();

        var consents = new List<global::EBOS.CRM.Domain.Entities.CRM.CustomerConsent>();
        for (var i = 0; i < 80; i++)
        {
            var when = DateTime.UtcNow.AddMinutes(-i);
            consents.Add(global::EBOS.CRM.Domain.Entities.CRM.CustomerConsent.Create(1, customer.Id, "MARKETING_EMAIL",
                true, when, "stress", null));
            consents.Add(global::EBOS.CRM.Domain.Entities.CRM.CustomerConsent.Create(1, customer.Id,
                "PRODUCT_UPDATES_SMS", true, when, "stress", null));
        }

        db.CustomerConsents.AddRange(consents);
        await db.SaveChangesAsync();

        return customer.Id;
    }
}
