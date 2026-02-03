using EBOS.CRM.ApiTests.Fixtures.EntitiesFactories.CRM;
using EBOS.CRM.Domain.Entities.CRM;

namespace EBOS.CRM.ApiTests.Fixtures.EntitiesFactories.CRM;

public class TaxInformationEntityFactoryTest
{
    public static TaxInformation CreateValidTaxInformation(
        string taxName = "VAT",
        string taxIdentificationNumber = "VAT-123",
        long customerId = 1)
    {
        return new TaxInformation
        {
            TaxName = taxName,
            TaxIdentificationNumber = taxIdentificationNumber,
            CustomerId = customerId
        };
    }
    [Fact]
    public void CreateValidTaxInformation_Defaults_AreSet()
    {
        var entity = CreateValidTaxInformation();

        Assert.NotNull(entity);
        Assert.Equal("VAT", entity.TaxName);
        Assert.Equal("VAT-123", entity.TaxIdentificationNumber);
        Assert.Equal(1, entity.CustomerId);
    }

    [Fact]
    public void CreateValidTaxInformation_CustomValues_AreApplied()
    {
        var entity = CreateValidTaxInformation(
            taxName: "IVA",
            taxIdentificationNumber: "ES123",
            customerId: 2);

        Assert.Equal("IVA", entity.TaxName);
        Assert.Equal("ES123", entity.TaxIdentificationNumber);
        Assert.Equal(2, entity.CustomerId);
    }
}


