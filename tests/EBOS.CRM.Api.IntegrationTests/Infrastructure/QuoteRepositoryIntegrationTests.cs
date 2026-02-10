using EBOS.CRM.Domain.Entities;
using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Infrastructure.Repositories.Concrete.CRM;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace EBOS.CRM.Api.IntegrationTests.Infrastructure;

public class QuoteRepositoryIntegrationTests
{
    [Fact]
    public async Task QuoteRepository_CRUD_Works()
    {
        using var context = SqliteCrmContextFactory.Create();
        var status = await EnsureStatusAsync(context);
        var customer = await EnsureCustomerAsync(context, status.Id);
        var stage = await EnsureOpportunityStageAsync(context);
        var opportunity = await EnsureOpportunityAsync(context, customer.Id, stage.Id);

        var repository = new QuoteRepository(context);

        var quote = new Quote
        {
            TenantId = 1,
            OpportunityId = opportunity.Id,
            Status = "Draft",
            ReferenceNumber = "Q-1001",
            SubtotalAmount = 10000m,
            DiscountAmount = 0m,
            TotalAmount = 10000m,
            Notes = "Initial",
            CreatedAt = DateTime.UtcNow,
            CreatedBy = 1
        };

        await repository.AddAsync(quote);
        await repository.SaveChangesAsync();

        var loaded = await repository.GetByIdAsync(quote.Id);
        loaded.Should().NotBeNull();
        loaded!.Status.Should().Be("Draft");

        loaded.Status = "Sent";
        await repository.UpdateAsync(loaded);
        await repository.SaveChangesAsync();

        var updated = await repository.GetByIdAsync(quote.Id);
        updated!.Status.Should().Be("Sent");

        await repository.DeleteAsync(updated);
        await repository.SaveChangesAsync();

        var deleted = await repository.GetByIdAsync(quote.Id);
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

    private static async Task<Opportunity> EnsureOpportunityAsync(DbContext context, long customerId, long stageId)
    {
        var existing = await context.Set<Opportunity>().FirstOrDefaultAsync();
        if (existing != null)
        {
            return existing;
        }

        var opportunity = new Opportunity
        {
            TenantId = 1,
            Name = "Deal A",
            StageId = stageId,
            OwnerUserId = 10,
            CustomerId = customerId,
            ExpectedCloseDate = DateTime.UtcNow.AddDays(30),
            Amount = 10000m,
            Probability = 0.5m
        };
        context.Add(opportunity);
        await context.SaveChangesAsync();
        return opportunity;
    }
}
