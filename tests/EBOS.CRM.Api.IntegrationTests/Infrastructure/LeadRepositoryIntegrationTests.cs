using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Infrastructure.Repositories.Concrete.CRM;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace EBOS.CRM.Api.IntegrationTests.Infrastructure;

public class LeadRepositoryIntegrationTests
{
    [Fact]
    public async Task LeadRepository_CRUD_Works()
    {
        using var context = SqliteCrmContextFactory.Create();
        var repository = new LeadRepository(context);

        var lead = new Lead
        {
            TenantId = 1,
            Source = "Web",
            Status = "New",
            OwnerUserId = 10,
            CompanyName = "Acme",
            ContactName = "Jane Doe",
            Email = "lead@acme.com",
            Phone = "1234567890",
            EstimatedValue = 1000m,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = 1
        };

        await repository.AddAsync(lead);
        await repository.SaveChangesAsync();

        var loaded = await repository.GetByIdAsync(lead.Id);
        loaded.Should().NotBeNull();
        loaded!.CompanyName.Should().Be("Acme");

        loaded.Status = "Qualified";
        await repository.UpdateAsync(loaded);
        await repository.SaveChangesAsync();

        var updated = await repository.GetByIdAsync(lead.Id);
        updated!.Status.Should().Be("Qualified");

        await repository.DeleteAsync(updated);
        await repository.SaveChangesAsync();

        var deleted = await repository.GetByIdAsync(lead.Id);
        deleted.Should().BeNull();
    }
}
