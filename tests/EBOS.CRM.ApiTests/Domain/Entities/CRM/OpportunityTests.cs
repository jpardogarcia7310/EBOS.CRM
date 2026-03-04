using EBOS.CRM.Domain.Entities.CRM;

namespace EBOS.CRM.ApiTests.Domain.Entities.CRM;

public class OpportunityTests
{
    [Fact]
    public void Opportunity_QuotesCollection_IsInitialized()
    {
        var entity = new Opportunity();
        Assert.NotNull(entity.Quotes);
        Assert.Empty(entity.Quotes);
    }

    [Fact]
    public void Opportunity_AllowsCoreProperties_Assignment()
    {
        var now = DateTime.UtcNow;
        var entity = new Opportunity
        {
            TenantId = 1,
            Name = "Upsell Q1",
            StageId = 2,
            OwnerUserId = 10,
            CustomerId = 100,
            ExpectedCloseDate = now.AddDays(30),
            Amount = 15000m,
            Probability = 0.45m,
            Source = "LEAD",
            SourceLeadId = 50,
            CloseReason = null,
            CreatedAt = now,
            CreatedBy = 99
        };

        Assert.Equal("Upsell Q1", entity.Name);
        Assert.Equal(15000m, entity.Amount);
        Assert.Equal(0.45m, entity.Probability);
        Assert.Equal(50, entity.SourceLeadId);
    }
}
