using EBOS.CRM.Domain.Entities.CRM;

namespace EBOS.CRM.ApiTests.Domain.Entities.CRM;

public class QuoteTests
{
    [Fact]
    public void Quote_AllowsCoreProperties_Assignment()
    {
        var now = DateTime.UtcNow;
        var entity = new Quote
        {
            TenantId = 1,
            OpportunityId = 10,
            Status = "DRAFT",
            ReferenceNumber = "Q-2026-001",
            SubtotalAmount = 1000m,
            DiscountAmount = 100m,
            TotalAmount = 900m,
            ValidUntil = now.AddDays(10),
            Notes = "Initial quote",
            CreatedAt = now,
            CreatedBy = 99
        };

        Assert.Equal("DRAFT", entity.Status);
        Assert.Equal("Q-2026-001", entity.ReferenceNumber);
        Assert.Equal(900m, entity.TotalAmount);
    }
}
