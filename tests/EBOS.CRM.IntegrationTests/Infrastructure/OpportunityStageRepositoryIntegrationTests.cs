using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Infrastructure.Repositories.Concrete.CRM;
using FluentAssertions;

namespace EBOS.CRM.IntegrationTests.Infrastructure;

public class OpportunityStageRepositoryIntegrationTests
{
    [Fact]
    public async Task OpportunityStageRepository_CRUD_Works()
    {
        await using var context = SqliteCrmContextFactory.Create();
        var repository = new OpportunityStageRepository(context);

        var stage = new OpportunityStage
        {
            TenantId = 1,
            Name = "Prospecting",
            Order = 1,
            DefaultProbability = 0.1m,
            IsClosed = false,
            IsWon = false
        };

        await repository.AddAsync(stage);
        await repository.SaveChangesAsync();

        var loaded = await repository.GetByIdAsync(stage.Id);
        loaded.Should().NotBeNull();
        loaded!.Name.Should().Be("Prospecting");

        loaded.Name = "Qualified";
        await repository.UpdateAsync(loaded);
        await repository.SaveChangesAsync();

        var updated = await repository.GetByIdAsync(stage.Id);
        updated!.Name.Should().Be("Qualified");

        await repository.DeleteAsync(updated);
        await repository.SaveChangesAsync();

        var deleted = await repository.GetByIdAsync(stage.Id);
        deleted.Should().BeNull();
    }
}
