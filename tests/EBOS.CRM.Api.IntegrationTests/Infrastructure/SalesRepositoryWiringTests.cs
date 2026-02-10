using EBOS.CRM.Api.IntegrationTests.Infrastructure;
using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace EBOS.CRM.Api.IntegrationTests.Infrastructure;

public class SalesRepositoryWiringTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    [Fact]
    public async Task Lead_Repository_Allows_Basic_Crud()
    {
        using var scope = factory.Services.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<ILeadRepository>();

        repository.Should().NotBeNull();

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

        lead.Id.Should().BeGreaterThan(0);

        var fetched = await repository.GetByIdAsync(lead.Id);
        fetched.Should().NotBeNull();
        fetched!.CompanyName.Should().Be("Acme Corp");

        fetched.Phone = "5551234567";
        await repository.UpdateAsync(fetched);
        await repository.SaveChangesAsync();

        var updated = await repository.GetByIdAsync(lead.Id);
        updated!.Phone.Should().Be("5551234567");

        await repository.DeleteAsync(updated);
        await repository.SaveChangesAsync();

        var deleted = await repository.GetByIdAsync(lead.Id);
        deleted.Should().BeNull();
    }

    [Fact]
    public async Task Opportunity_Repository_Allows_Basic_Crud()
    {
        using var scope = factory.Services.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IOpportunityRepository>();
        var stageRepository = scope.ServiceProvider.GetRequiredService<IOpportunityStageRepository>();

        repository.Should().NotBeNull();

        var stage = (await stageRepository.GetAllAsync()).First();
        var customerId = await EnsureCustomerAsync(scope.ServiceProvider);

        var opportunity = new Opportunity
        {
            TenantId = 1,
            Name = "Upgrade Plan",
            StageId = stage.Id,
            OwnerUserId = 1,
            CustomerId = customerId,
            Amount = 1500m,
            Probability = stage.DefaultProbability
        };

        await repository.AddAsync(opportunity);
        await repository.SaveChangesAsync();

        opportunity.Id.Should().BeGreaterThan(0);

        var fetched = await repository.GetByIdAsync(opportunity.Id);
        fetched.Should().NotBeNull();
        fetched!.Name.Should().Be("Upgrade Plan");

        fetched.Amount = 1750m;
        await repository.UpdateAsync(fetched);
        await repository.SaveChangesAsync();

        var updated = await repository.GetByIdAsync(opportunity.Id);
        updated!.Amount.Should().Be(1750m);

        await repository.DeleteAsync(updated);
        await repository.SaveChangesAsync();

        var deleted = await repository.GetByIdAsync(opportunity.Id);
        deleted.Should().BeNull();
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
            Code = "CUST-0001",
            Email = "customer@acme.test",
            Phone = "5550001111",
            StatusId = 1
        };

        await customerRepository.AddAsync(customer);
        await customerRepository.SaveChangesAsync();
        return customer.Id;
    }
}
