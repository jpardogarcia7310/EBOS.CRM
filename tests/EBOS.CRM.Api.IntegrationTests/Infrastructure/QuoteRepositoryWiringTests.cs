using EBOS.CRM.Api.IntegrationTests.Infrastructure;
using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace EBOS.CRM.Api.IntegrationTests.Infrastructure;

public class QuoteRepositoryWiringTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    [Fact]
    public async Task Quote_Repository_Allows_Basic_Crud()
    {
        using var scope = factory.Services.CreateScope();
        var quoteRepository = scope.ServiceProvider.GetRequiredService<IQuoteRepository>();

        quoteRepository.Should().NotBeNull();

        var opportunityId = await EnsureOpportunityAsync(scope.ServiceProvider);

        var quote = new Quote
        {
            TenantId = 1,
            OpportunityId = opportunityId,
            Status = "Draft",
            ReferenceNumber = "Q-0001",
            SubtotalAmount = 1000m,
            DiscountAmount = 100m,
            TotalAmount = 900m,
            Notes = "Initial quote"
        };

        await quoteRepository.AddAsync(quote);
        await quoteRepository.SaveChangesAsync();

        quote.Id.Should().BeGreaterThan(0);

        var fetched = await quoteRepository.GetByIdAsync(quote.Id);
        fetched.Should().NotBeNull();
        fetched!.ReferenceNumber.Should().Be("Q-0001");

        fetched.Status = "Sent";
        await quoteRepository.UpdateAsync(fetched);
        await quoteRepository.SaveChangesAsync();

        var updated = await quoteRepository.GetByIdAsync(quote.Id);
        updated!.Status.Should().Be("Sent");

        await quoteRepository.DeleteAsync(updated);
        await quoteRepository.SaveChangesAsync();

        var deleted = await quoteRepository.GetByIdAsync(quote.Id);
        deleted.Should().BeNull();
    }

    private static async Task<long> EnsureOpportunityAsync(IServiceProvider services)
    {
        var opportunityRepository = services.GetRequiredService<IOpportunityRepository>();
        var opportunities = await opportunityRepository.GetAllAsync();
        var existing = opportunities.FirstOrDefault();
        if (existing != null)
        {
            return existing.Id;
        }

        var stageRepository = services.GetRequiredService<IOpportunityStageRepository>();
        var stage = (await stageRepository.GetAllAsync()).First();

        var customerId = await EnsureCustomerAsync(services);

        var opportunity = new Opportunity
        {
            TenantId = 1,
            Name = "Quote Test Opportunity",
            StageId = stage.Id,
            OwnerUserId = 1,
            CustomerId = customerId,
            Amount = 1000m,
            Probability = stage.DefaultProbability
        };

        await opportunityRepository.AddAsync(opportunity);
        await opportunityRepository.SaveChangesAsync();
        return opportunity.Id;
    }

    private static async Task<long> EnsureCustomerAsync(IServiceProvider services)
    {
        var customerRepository = services.GetRequiredService<ICustomerRepository>();
        var customers = await customerRepository.GetAllAsync();
        var existing = customers.FirstOrDefault();
        if (existing != null)
        {
            return existing.Id;
        }

        var customer = new Customer
        {
            TenantId = 1,
            Code = "CUST-Q-01",
            Email = "quote.customer@acme.test",
            Phone = "5550002222",
            StatusId = 1
        };

        await customerRepository.AddAsync(customer);
        await customerRepository.SaveChangesAsync();
        return customer.Id;
    }
}
