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

public class QuoteRepositoryNegativeTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    [Fact]
    public async Task Quote_Save_Succeeds_When_TenantId_Matches_And_Opportunity_Exists()
    {
        using var scope = factory.Services.CreateScope();
        var context = CreateTenantContext(scope.ServiceProvider, 1);
        var repository = new QuoteRepository(context);

        var (customerId, stageId) = await EnsureOpportunityDependenciesAsync(context);
        var opportunityId = await EnsureOpportunityAsync(context, customerId, stageId);

        var quote = new Quote
        {
            TenantId = 1,
            OpportunityId = opportunityId,
            Status = "Draft",
            SubtotalAmount = 1000m,
            DiscountAmount = 0m,
            TotalAmount = 1000m
        };

        await repository.AddAsync(quote);
        await repository.SaveChangesAsync();

        var saved = await repository.GetByIdAsync(quote.Id);
        saved.Should().NotBeNull();
    }

    [Fact]
    public async Task Quote_Save_Throws_When_TenantId_Mismatch()
    {
        using var scope = factory.Services.CreateScope();
        var context = CreateTenantContext(scope.ServiceProvider, 1);
        var repository = new QuoteRepository(context);

        var quote = new Quote
        {
            TenantId = 2,
            OpportunityId = 1,
            Status = "Draft",
            SubtotalAmount = 1000m,
            DiscountAmount = 0m,
            TotalAmount = 1000m
        };

        await repository.AddAsync(quote);

        var act = async () => await repository.SaveChangesAsync();
        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task Quote_Save_Throws_When_Opportunity_Missing_On_Relational()
    {
        using var scope = factory.Services.CreateScope();
        var context = CreateTenantContext(scope.ServiceProvider, 1);
        if (!context.Database.IsRelational())
        {
            return;
        }

        var repository = new QuoteRepository(context);

        var quote = new Quote
        {
            TenantId = 1,
            OpportunityId = 999999,
            Status = "Draft",
            SubtotalAmount = 1000m,
            DiscountAmount = 0m,
            TotalAmount = 1000m
        };

        await repository.AddAsync(quote);

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

    private static async Task<(long CustomerId, long StageId)> EnsureOpportunityDependenciesAsync(CrmDbContext context)
    {
        var customerRepository = new CustomerRepository(context);
        var status = await context.Set<EBOS.CRM.Domain.Entities.EBOS.Status>().FirstOrDefaultAsync();
        if (status == null)
        {
            status = new EBOS.CRM.Domain.Entities.EBOS.Status { Description = "Active" };
            context.Add(status);
            await context.SaveChangesAsync();
        }

        var customer = (await customerRepository.GetAllAsync()).FirstOrDefault();
        if (customer == null)
        {
            customer = new Customer
            {
                TenantId = 1,
                Code = "CUST-NEG-01",
                Email = "customer.neg@acme.test",
                Phone = "5550001111",
                StatusId = status.Id
            };

            await customerRepository.AddAsync(customer);
            await customerRepository.SaveChangesAsync();
        }

        var stageRepository = new OpportunityStageRepository(context);
        var stage = (await stageRepository.GetAllAsync()).FirstOrDefault();
        if (stage == null)
        {
            stage = new OpportunityStage
            {
                TenantId = 1,
                Name = "Prospección",
                Order = 1,
                DefaultProbability = 0.1m,
                IsClosed = false,
                IsWon = false
            };

            await stageRepository.AddAsync(stage);
            await stageRepository.SaveChangesAsync();
        }

        return (customer.Id, stage.Id);
    }

    private static async Task<long> EnsureOpportunityAsync(CrmDbContext context, long customerId, long stageId)
    {
        var opportunityRepository = new OpportunityRepository(context);
        var existing = (await opportunityRepository.GetAllAsync()).FirstOrDefault();
        if (existing != null)
        {
            return existing.Id;
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

        await opportunityRepository.AddAsync(opportunity);
        await opportunityRepository.SaveChangesAsync();
        return opportunity.Id;
    }

    private sealed class TestTenantContext(long tenantId) : ITenantContext
    {
        public long TenantId { get; } = tenantId;
    }
}
