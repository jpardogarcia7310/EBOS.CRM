using EBOS.CRM.Domain.Entities.CRM;

namespace EBOS.CRM.ApiTests.Fixtures.EntitiesFactories.CRM;

public class OpportunityEntityFactoryTest
{
    private static Opportunity CreateValidOpportunity(string name = "Deal A", long stageId = 1, long ownerUserId = 2,
        long customerId = 3, decimal amount = 1000m, decimal probability = 0.5m)
    {
        return new Opportunity
        {
            TenantId = 1,
            Name = name,
            StageId = stageId,
            OwnerUserId = ownerUserId,
            CustomerId = customerId,
            ExpectedCloseDate = DateTime.UtcNow.AddDays(30),
            Amount = amount,
            Probability = probability
        };
    }

    [Fact]
    public void CreateValidOpportunity_Defaults_AreSet()
    {
        var entity = CreateValidOpportunity();

        Assert.NotNull(entity);
        Assert.Equal("Deal A", entity.Name);
        Assert.Equal(1, entity.StageId);
        Assert.Equal(2, entity.OwnerUserId);
        Assert.Equal(3, entity.CustomerId);
        Assert.Equal(1000m, entity.Amount);
        Assert.Equal(0.5m, entity.Probability);
    }

    [Fact]
    public void CreateValidOpportunity_CustomValues_AreApplied()
    {
        var entity = CreateValidOpportunity(
            name: "Big Deal",
            stageId: 5,
            ownerUserId: 10,
            customerId: 20,
            amount: 25000m,
            probability: 0.75m);

        Assert.Equal("Big Deal", entity.Name);
        Assert.Equal(5, entity.StageId);
        Assert.Equal(10, entity.OwnerUserId);
        Assert.Equal(20, entity.CustomerId);
        Assert.Equal(25000m, entity.Amount);
        Assert.Equal(0.75m, entity.Probability);
    }
}
