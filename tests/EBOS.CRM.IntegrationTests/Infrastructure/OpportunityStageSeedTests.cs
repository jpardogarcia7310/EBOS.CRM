using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace EBOS.CRM.IntegrationTests.Infrastructure;

public class OpportunityStageSeedTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    [Fact]
    public async Task OpportunityStageRepository_Is_Wired_And_Seeds_Are_Present()
    {
        using var scope = factory.Services.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IOpportunityStageRepository>();

        repository.Should().NotBeNull();

        var stages = await repository.GetAllAsync();
        stages.Should().NotBeNull();
        stages.Count.Should().BeGreaterThan(5);

        var names = stages.Select(s => s.Name).ToList();
        names.Should().Contain(new[]
        {
            "Prospección",
            "Calificado",
            "Propuesta",
            "Negociación",
            "Cerrado Ganado",
            "Cerrado Perdido"
        });
    }
}
