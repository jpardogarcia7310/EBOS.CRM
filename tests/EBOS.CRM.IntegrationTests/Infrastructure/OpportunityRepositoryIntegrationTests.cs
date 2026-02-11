using EBOS.CRM.Domain.Entities;
using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Infrastructure.Repositories.Concrete.CRM;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace EBOS.CRM.IntegrationTests.Infrastructure;

public class OpportunityRepositoryIntegrationTests
{
    [Fact]
    public async Task OpportunityRepository_CRUD_Works()
    {
        await using var context = SqliteCrmContextFactory.Create();
        var status = await EnsureStatusAsync(context);
        var customer = await EnsureCustomerAsync(context, status.Id);
        var stage = await EnsureOpportunityStageAsync(context);

        var repository = new OpportunityRepository(context);

        var opportunity = new Opportunity
        {
            TenantId = 1,
            Name = "Deal A",
            StageId = stage.Id,
            OwnerUserId = 10,
            CustomerId = customer.Id,
            ExpectedCloseDate = DateTime.UtcNow.AddDays(30),
            Amount = 10000m,
            Probability = 0.5m,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = 1
        };

        await repository.AddAsync(opportunity);
        await repository.SaveChangesAsync();

        var loaded = await repository.GetByIdAsync(opportunity.Id);
        loaded.Should().NotBeNull();
        loaded!.Name.Should().Be("Deal A");

        loaded.Amount = 12000m;
        await repository.UpdateAsync(loaded);
        await repository.SaveChangesAsync();

        var updated = await repository.GetByIdAsync(opportunity.Id);
        updated!.Amount.Should().Be(12000m);

        await repository.DeleteAsync(updated);
        await repository.SaveChangesAsync();

        var deleted = await repository.GetByIdAsync(opportunity.Id);
        deleted.Should().BeNull();
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

    private static async Task<Customer> EnsureCustomerAsync(DbContext context, long statusId)
    {
        var existing = await context.Set<Customer>().FirstOrDefaultAsync();
        if (existing != null)
        {
            return existing;
        }

        var customer = new Customer
        {
            TenantId = 1,
            Code = "CUST-001",
            Email = "customer@test.com",
            Phone = "1234567890",
            StatusId = statusId
        };
        context.Add(customer);
        await context.SaveChangesAsync();
        return customer;
    }

    private static async Task<OpportunityStage> EnsureOpportunityStageAsync(DbContext context)
    {
        var existing = await context.Set<OpportunityStage>().FirstOrDefaultAsync();
        if (existing != null)
        {
            return existing;
        }

        var stage = new OpportunityStage
        {
            TenantId = 1,
            Name = "Prospecting",
            Order = 1,
            DefaultProbability = 0.1m,
            IsClosed = false,
            IsWon = false
        };
        context.Add(stage);
        await context.SaveChangesAsync();
        return stage;
    }
}
