using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Entities.EBOS;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM.Models;
using EBOS.CRM.Infrastructure.Options;
using EBOS.CRM.Infrastructure.Repositories.Concrete.CRM;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace EBOS.CRM.IntegrationTests.Infrastructure;

public class CustomerDedupeRepositoryIntegrationTests
{
    [Fact]
    public async Task FindDuplicates_ExcludesErasedCustomers()
    {
        await using var context = SqliteCrmContextFactory.Create();
        var status = await EnsureStatusAsync(context);

        var active = new Customer
        {
            TenantId = 1,
            Code = "DED-ACTIVE",
            Email = "dup@example.com",
            Phone = "111222333444",
            StatusId = status.Id,
            Erased = false
        };
        var erased = new Customer
        {
            TenantId = 1,
            Code = "DED-ERASED",
            Email = "dup@example.com",
            Phone = "999888777666",
            StatusId = status.Id,
            Erased = true
        };

        context.Customers.AddRange(active, erased);
        await context.SaveChangesAsync();

        var repository = new CustomerDedupeRepository(
            context,
            Options.Create(new CustomerDedupeOptions()));

        var criteria = new CustomerDedupeCriteria(1, "dup@example.com", null, null, null);
        var matches = await repository.FindDuplicatesAsync(criteria, 1, 10);

        matches.Should().ContainSingle(x => x.CustomerId == active.Id);
        matches.Should().NotContain(x => x.CustomerId == erased.Id);
    }

    [Fact]
    public async Task FindDuplicates_UsesPhoneSuffixFallback_WhenEnabled()
    {
        await using var context = SqliteCrmContextFactory.Create();
        var status = await EnsureStatusAsync(context);

        var customer = new Customer
        {
            TenantId = 1,
            Code = "DED-PHONE",
            Email = "phone@example.com",
            Phone = "123456789012",
            StatusId = status.Id,
            Erased = false
        };

        context.Customers.Add(customer);
        await context.SaveChangesAsync();

        var options = new CustomerDedupeOptions
        {
            EnablePhoneSuffixFallback = true,
            PhoneSuffixLength = 8,
            PhoneApproxWeight = 10,
            MinScore = 10
        };

        var repository = new CustomerDedupeRepository(context, Options.Create(options));
        var criteria = new CustomerDedupeCriteria(1, null, "000056789012", null, null);

        var matches = await repository.FindDuplicatesAsync(criteria, 1, 10);

        matches.Should().ContainSingle(x => x.CustomerId == customer.Id);
        matches.Single().MatchReason.Should().Contain("PhoneApprox");
    }

    private static async Task<Status> EnsureStatusAsync(DbContext context)
    {
        var existing = await context.Set<Status>().FirstOrDefaultAsync();
        if (existing != null)
        {
            return existing;
        }

        var status = new Status { Description = "Active" };
        context.Add(status);
        await context.SaveChangesAsync();
        return status;
    }
}
