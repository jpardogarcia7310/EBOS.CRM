using EBOS.CRM.Domain.Entities.CRM;

namespace EBOS.CRM.ApiTests.Domain.Entities.CRM;

public class LeadTests
{
    [Fact]
    public void Lead_AllowsCoreProperties_Assignment()
    {
        var now = DateTime.UtcNow;
        var lead = new Lead
        {
            TenantId = 1,
            Source = "WEB",
            Status = "NEW",
            OwnerUserId = 7,
            CompanyName = "ACME",
            ContactName = "John Doe",
            Email = "john@acme.com",
            Phone = "3000000000",
            EstimatedValue = 1000m,
            Notes = "hot",
            CreatedAt = now,
            CreatedBy = 99,
            UpdatedAt = now,
            UpdatedBy = 100
        };

        Assert.Equal(1, lead.TenantId);
        Assert.Equal("WEB", lead.Source);
        Assert.Equal("NEW", lead.Status);
        Assert.Equal("ACME", lead.CompanyName);
        Assert.Equal(1000m, lead.EstimatedValue);
    }

    [Fact]
    public void Lead_ConvertedOpportunity_CanBeNullOrAssigned()
    {
        var lead = new Lead { TenantId = 1, Source = "WEB", Status = "NEW", OwnerUserId = 1, CompanyName = "A", ContactName = "B", Email = "a@b.com", Phone = "1" };
        Assert.Null(lead.ConvertedOpportunityId);

        lead.ConvertedOpportunityId = 42;
        Assert.Equal(42, lead.ConvertedOpportunityId);
    }
}
