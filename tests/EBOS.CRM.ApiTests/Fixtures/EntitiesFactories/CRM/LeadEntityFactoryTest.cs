using EBOS.CRM.Domain.Entities.CRM;

namespace EBOS.CRM.ApiTests.Fixtures.EntitiesFactories.CRM;

public class LeadEntityFactoryTest
{
    private static Lead CreateValidLead(string source = "Web", string status = "New", long ownerUserId = 1,
        string companyName = "Acme", string contactName = "Jane Doe", string email = "lead@acme.com",
        string phone = "123456", decimal? estimatedValue = 1000m)
    {
        return new Lead
        {
            TenantId = 1,
            Source = source,
            Status = status,
            OwnerUserId = ownerUserId,
            CompanyName = companyName,
            ContactName = contactName,
            Email = email,
            Phone = phone,
            EstimatedValue = estimatedValue
        };
    }

    [Fact]
    public void CreateValidLead_Defaults_AreSet()
    {
        var entity = CreateValidLead();

        Assert.NotNull(entity);
        Assert.Equal("Web", entity.Source);
        Assert.Equal("New", entity.Status);
        Assert.Equal("Acme", entity.CompanyName);
        Assert.Equal("Jane Doe", entity.ContactName);
        Assert.Equal("lead@acme.com", entity.Email);
        Assert.Equal("123456", entity.Phone);
    }

    [Fact]
    public void CreateValidLead_CustomValues_AreApplied()
    {
        var entity = CreateValidLead(
            source: "Referral",
            status: "Qualified",
            ownerUserId: 10,
            companyName: "Contoso",
            contactName: "John Smith",
            email: "john@contoso.com",
            phone: "654321",
            estimatedValue: 5000m);

        Assert.Equal("Referral", entity.Source);
        Assert.Equal("Qualified", entity.Status);
        Assert.Equal(10, entity.OwnerUserId);
        Assert.Equal("Contoso", entity.CompanyName);
        Assert.Equal("John Smith", entity.ContactName);
        Assert.Equal("john@contoso.com", entity.Email);
        Assert.Equal("654321", entity.Phone);
        Assert.Equal(5000m, entity.EstimatedValue);
    }
}
