using EBOS.CRM.Domain.Entities.CRM;

namespace EBOS.CRM.ApiTests.Fixtures.EntitiesFactories.CRM;

public class CorporateCustomerEntityFactoryTest
{
    private static CorporateCustomer CreateValidCorporateCustomer(string code = "CORP-001",
        string email = "corp@example.com", string phone = "123", DateTime? createdAt = null, long statusId = 1,
        string legalName = "Corp SA", string taxIdentification = "TAX123")
    {
        return new CorporateCustomer
        {
            Code = code,
            Email = email,
            Phone = phone,
            CreatedAt = createdAt ?? DateTime.UtcNow,
            StatusId = statusId,
            LegalName = legalName,
            TaxIdentification = taxIdentification
        };
    }

    [Fact]
    public void CreateValidCorporateCustomer_Defaults_AreSet()
    {
        var entity = CreateValidCorporateCustomer();

        Assert.NotNull(entity);
        Assert.Equal("CORP-001", entity.Code);
        Assert.Equal("corp@example.com", entity.Email);
        Assert.Equal("123", entity.Phone);
        Assert.Equal(1, entity.StatusId);
        Assert.Equal("Corp SA", entity.LegalName);
        Assert.Equal("TAX123", entity.TaxIdentification);
    }

    [Fact]
    public void CreateValidCorporateCustomer_CustomValues_AreApplied()
    {
        var date = new DateTime(2024, 1, 1);
        var entity = CreateValidCorporateCustomer(
            code: "CORP-999",
            email: "x@y.com",
            phone: "999",
            createdAt: date,
            statusId: 2,
            legalName: "Custom Corp",
            taxIdentification: "TAX999");

        Assert.Equal("CORP-999", entity.Code);
        Assert.Equal("x@y.com", entity.Email);
        Assert.Equal("999", entity.Phone);
        Assert.Equal(date, entity.CreatedAt);
        Assert.Equal(2, entity.StatusId);
        Assert.Equal("Custom Corp", entity.LegalName);
        Assert.Equal("TAX999", entity.TaxIdentification);
    }
}


