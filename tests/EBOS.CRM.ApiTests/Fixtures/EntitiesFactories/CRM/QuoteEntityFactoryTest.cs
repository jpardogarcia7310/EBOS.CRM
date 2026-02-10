using EBOS.CRM.Domain.Entities.CRM;

namespace EBOS.CRM.ApiTests.Fixtures.EntitiesFactories.CRM;

public class QuoteEntityFactoryTest
{
    private static Quote CreateValidQuote(long opportunityId = 1, string status = "Draft",
        decimal subtotal = 1000m, decimal discount = 0m, decimal total = 1000m)
    {
        return new Quote
        {
            TenantId = 1,
            OpportunityId = opportunityId,
            Status = status,
            ReferenceNumber = "Q-1001",
            SubtotalAmount = subtotal,
            DiscountAmount = discount,
            TotalAmount = total
        };
    }

    [Fact]
    public void CreateValidQuote_Defaults_AreSet()
    {
        var entity = CreateValidQuote();

        Assert.NotNull(entity);
        Assert.Equal(1, entity.OpportunityId);
        Assert.Equal("Draft", entity.Status);
        Assert.Equal("Q-1001", entity.ReferenceNumber);
        Assert.Equal(1000m, entity.SubtotalAmount);
        Assert.Equal(0m, entity.DiscountAmount);
        Assert.Equal(1000m, entity.TotalAmount);
    }

    [Fact]
    public void CreateValidQuote_CustomValues_AreApplied()
    {
        var entity = CreateValidQuote(
            opportunityId: 5,
            status: "Sent",
            subtotal: 2000m,
            discount: 100m,
            total: 1900m);

        Assert.Equal(5, entity.OpportunityId);
        Assert.Equal("Sent", entity.Status);
        Assert.Equal(2000m, entity.SubtotalAmount);
        Assert.Equal(100m, entity.DiscountAmount);
        Assert.Equal(1900m, entity.TotalAmount);
    }
}
