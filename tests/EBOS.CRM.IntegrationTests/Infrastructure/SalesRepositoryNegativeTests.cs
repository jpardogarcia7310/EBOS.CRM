using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Interfaces.Services;
using EBOS.CRM.Domain.Interfaces.Services.EBOS;
using EBOS.CRM.Infrastructure.Options;
using EBOS.CRM.Infrastructure.Persistence;
using EBOS.CRM.Infrastructure.Repositories.Concrete.CRM;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace EBOS.CRM.IntegrationTests.Infrastructure;

public class SalesRepositoryNegativeTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    [Fact]
    public async Task Lead_Save_Succeeds_When_TenantId_Matches()
    {
        using var scope = factory.Services.CreateScope();
        var context = CreateTenantContext(scope.ServiceProvider, 1);
        var repository = new LeadRepository(context);

        var lead = new Lead
        {
            TenantId = 1,
            Source = "Web",
            Status = "New",
            OwnerUserId = 1,
            CompanyName = "Acme Corp",
            ContactName = "Jane Doe",
            Email = "jane.doe@acme.test",
            Phone = "1234567890"
        };

        await repository.AddAsync(lead);
        await repository.SaveChangesAsync();

        var saved = await repository.GetByIdAsync(lead.Id);
        saved.Should().NotBeNull();
    }

    [Fact]
    public async Task Opportunity_Save_Succeeds_When_TenantId_Matches_And_Dependencies_Exist()
    {
        using var scope = factory.Services.CreateScope();
        var context = CreateTenantContext(scope.ServiceProvider, 1);
        var repository = new OpportunityRepository(context);

        var customerId = await EnsureCustomerAsync(context);
        var stageId = await EnsureStageAsync(context);

        var opportunity = new Opportunity
        {
            TenantId = 1,
            Name = "Upgrade Plan",
            StageId = stageId,
            OwnerUserId = 1,
            CustomerId = customerId,
            Amount = 1000m,
            Probability = 0.5m
        };

        await repository.AddAsync(opportunity);
        await repository.SaveChangesAsync();

        var saved = await repository.GetByIdAsync(opportunity.Id);
        saved.Should().NotBeNull();
    }

    [Fact]
    public async Task Lead_Save_Throws_When_TenantId_Mismatch()
    {
        using var scope = factory.Services.CreateScope();
        var context = CreateTenantContext(scope.ServiceProvider, 1);
        var repository = new LeadRepository(context);

        var lead = new Lead
        {
            TenantId = 2,
            Source = "Web",
            Status = "New",
            OwnerUserId = 1,
            CompanyName = "Acme Corp",
            ContactName = "Jane Doe",
            Email = "jane.doe@acme.test",
            Phone = "1234567890"
        };

        await repository.AddAsync(lead);

        var act = async () => await repository.SaveChangesAsync();
        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task Opportunity_Save_Throws_When_TenantId_Mismatch()
    {
        using var scope = factory.Services.CreateScope();
        var context = CreateTenantContext(scope.ServiceProvider, 1);
        var repository = new OpportunityRepository(context);

        var opportunity = new Opportunity
        {
            TenantId = 2,
            Name = "Upgrade Plan",
            StageId = 1,
            OwnerUserId = 1,
            CustomerId = 1,
            Amount = 1000m,
            Probability = 0.5m
        };

        await repository.AddAsync(opportunity);

        var act = async () => await repository.SaveChangesAsync();
        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task Opportunity_Save_Throws_When_Stage_Missing_On_Relational()
    {
        using var scope = factory.Services.CreateScope();
        var context = CreateTenantContext(scope.ServiceProvider, 1);
        if (!context.Database.IsRelational())
        {
            return;
        }

        var repository = new OpportunityRepository(context);
        var customerId = await EnsureCustomerAsync(context);

        var opportunity = new Opportunity
        {
            TenantId = 1,
            Name = "Missing Stage",
            StageId = 999999,
            OwnerUserId = 1,
            CustomerId = customerId,
            Amount = 1000m,
            Probability = 0.5m
        };

        await repository.AddAsync(opportunity);

        var act = async () => await repository.SaveChangesAsync();
        await act.Should().ThrowAsync<DbUpdateException>();
    }

    [Fact]
    public async Task Opportunity_Save_Throws_When_Customer_Missing_On_Relational()
    {
        using var scope = factory.Services.CreateScope();
        var context = CreateTenantContext(scope.ServiceProvider, 1);
        if (!context.Database.IsRelational())
        {
            return;
        }

        var repository = new OpportunityRepository(context);
        var stageId = await EnsureStageAsync(context);

        var opportunity = new Opportunity
        {
            TenantId = 1,
            Name = "Missing Customer",
            StageId = stageId,
            OwnerUserId = 1,
            CustomerId = 999999,
            Amount = 1000m,
            Probability = 0.5m
        };

        await repository.AddAsync(opportunity);

        var act = async () => await repository.SaveChangesAsync();
        await act.Should().ThrowAsync<DbUpdateException>();
    }

    private static CrmDbContext CreateTenantContext(IServiceProvider services, long tenantId)
    {
        var options = services.GetRequiredService<DbContextOptions<CrmDbContext>>();
        var multiTenantOptions = services.GetService<IOptions<MultiTenantOptions>>();
        var tenantContext = new TestTenantContext(tenantId);
        return new CrmDbContext(options, tenantContext, multiTenantOptions);
    }

    private static async Task<long> EnsureCustomerAsync(CrmDbContext context)
    {
        var repository = new CustomerRepository(context);
        var customers = await repository.GetAllAsync();
        var existing = customers.FirstOrDefault();
        if (existing != null)
        {
            return existing.Id;
        }

        var customer = new Customer
        {
            TenantId = 1,
            Code = "CUST-NEG-01",
            Email = "customer.neg@acme.test",
            Phone = "5550001111",
            StatusId = 1
        };

        await repository.AddAsync(customer);
        await repository.SaveChangesAsync();
        return customer.Id;
    }

    private static async Task<long> EnsureStageAsync(CrmDbContext context)
    {
        var repository = new OpportunityStageRepository(context);
        var stages = await repository.GetAllAsync();
        var existing = stages.FirstOrDefault();
        if (existing != null)
        {
            return existing.Id;
        }

        var stage = new OpportunityStage
        {
            TenantId = 1,
            Name = "Prospección",
            Order = 1,
            DefaultProbability = 0.1m,
            IsClosed = false,
            IsWon = false
        };

        await repository.AddAsync(stage);
        await repository.SaveChangesAsync();
        return stage.Id;
    }

    private sealed class TestTenantContext(long tenantId) : ITenantContext
    {
        public long TenantId { get; } = tenantId;
    }
}
