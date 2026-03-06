using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Events;
using EBOS.CRM.Domain.Exceptions;

namespace EBOS.CRM.ApiTests.Domain;

public class EnterpriseDomainObservabilityGrowthAggregatesTests
{
    [Fact]
    public void Lead_StatusTransitions_AreMonotonic_AndEmitDeterministicEvents()
    {
        var lead = new Lead
        {
            TenantId = 1,
            Source = "web",
            Status = Lead.StatusNew,
            OwnerUserId = 2,
            CompanyName = "Acme",
            ContactName = "Jane",
            Email = "jane@acme.com",
            Phone = "3000000000"
        };

        lead.Qualify("ok");
        lead.Qualify("dup");
        lead.MarkConverted(10, "conv");

        var ex = Assert.Throws<DomainRuleViolationException>(() => lead.Disqualify("late"));
        Assert.Equal("DOMAIN_RULE_VIOLATION_LEAD_STATUS_TRANSITION", ex.Code);

        var emitted = lead.DequeueOperationalEvents();
        Assert.NotEmpty(emitted);
        Assert.All(emitted, e =>
            Assert.Equal(DomainOperationalEventCatalog.Classify(e.Name), e.Category));
    }

    [Fact]
    public void Opportunity_Close_IsIdempotent_AndEmitsDeterministicEvents()
    {
        var opportunity = new Opportunity
        {
            TenantId = 1
        };

        opportunity.ApplyUpdate("Opp", 1, 2, 3, DateTime.UtcNow.AddDays(30), 1000m, 0.5m, "lead", 5, null);
        opportunity.Close(stageId: 9, isWon: false, closeReason: "lost");
        opportunity.Close(stageId: 9, isWon: false, closeReason: "lost");

        var emitted = opportunity.DequeueOperationalEvents();
        Assert.Contains(emitted, e => e.Name == "OpportunityClosed");
        Assert.Contains(emitted, e => e.Name == "DomainCommandDeduplicated");
        Assert.All(emitted, e =>
            Assert.Equal(DomainOperationalEventCatalog.Classify(e.Name), e.Category));
    }

    [Fact]
    public void Quote_Invariants_RejectAmountMismatch_AndEmitBusinessStatusEvents()
    {
        var quote = new Quote { TenantId = 1 };

        var ex = Assert.Throws<DomainRuleViolationException>(() =>
            quote.ApplyUpdate(
                opportunityId: 1,
                status: "DRAFT",
                referenceNumber: "Q-1",
                subtotalAmount: 100,
                discountAmount: 20,
                totalAmount: 50,
                validUntil: DateTime.UtcNow.AddDays(10),
                notes: "n"));
        Assert.Equal("DOMAIN_RULE_VIOLATION_QUOTE_TOTAL_MISMATCH", ex.Code);

        quote.ApplyUpdate(1, "DRAFT", "Q-1", 100, 20, 80, DateTime.UtcNow.AddDays(10), "n");
        quote.SetStatus("DRAFT");

        var emitted = quote.DequeueOperationalEvents();
        Assert.Contains(emitted, e => e.Name == "QuoteStatusChanged");
        Assert.Contains(emitted, e => e.Name == "DomainCommandDeduplicated");
        Assert.All(emitted, e =>
            Assert.Equal(DomainOperationalEventCatalog.Classify(e.Name), e.Category));
    }
}
