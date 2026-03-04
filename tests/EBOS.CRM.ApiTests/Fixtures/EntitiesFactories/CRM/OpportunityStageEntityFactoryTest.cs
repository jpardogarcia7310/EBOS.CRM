using EBOS.CRM.Domain.Entities.CRM;

namespace EBOS.CRM.ApiTests.Fixtures.EntitiesFactories.CRM;

public class OpportunityStageEntityFactoryTest
{
    private static OpportunityStage CreateValidStage(string name = "Prospecting", int order = 1,
        decimal defaultProbability = 0.1m, bool isClosed = false, bool isWon = false)
    {
        return new OpportunityStage
        {
            TenantId = 1,
            Name = name,
            Order = order,
            DefaultProbability = defaultProbability,
            IsClosed = isClosed,
            IsWon = isWon
        };
    }

    [Fact]
    public void CreateValidStage_Defaults_AreSet()
    {
        var entity = CreateValidStage();

        Assert.NotNull(entity);
        Assert.Equal("Prospecting", entity.Name);
        Assert.Equal(1, entity.Order);
        Assert.Equal(0.1m, entity.DefaultProbability);
        Assert.False(entity.IsClosed);
        Assert.False(entity.IsWon);
    }

    [Fact]
    public void CreateValidStage_CustomValues_AreApplied()
    {
        var entity = CreateValidStage(
            name: "Won",
            order: 6,
            defaultProbability: 1m,
            isClosed: true,
            isWon: true);

        Assert.Equal("Won", entity.Name);
        Assert.Equal(6, entity.Order);
        Assert.Equal(1m, entity.DefaultProbability);
        Assert.True(entity.IsClosed);
        Assert.True(entity.IsWon);
    }
}
