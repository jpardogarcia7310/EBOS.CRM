using EBOS.CRM.ApiTests.Fixtures.EntitiesFactories.CRM;
using EBOS.CRM.Domain.Entities.CRM;

namespace EBOS.CRM.ApiTests.Fixtures.EntitiesFactories.CRM;

public class TaxInformationAddressEntityFactoryTest
{
    private static TaxInformationAddress CreateValidTaxInformationAddress(long taxInformationId = 1, long addressId = 1,
        bool isPrimary = true, DateTime? validFrom = null, DateTime? validTo = null, bool isCurrent = true)
    {
        return new TaxInformationAddress
        {
            TaxInformationId = taxInformationId,
            AddressId = addressId,
            IsPrimary = isPrimary,
            ValidFrom = validFrom ?? DateTime.UtcNow.AddDays(-1),
            ValidTo = validTo,
            IsCurrent = isCurrent
        };
    }

    [Fact]
    public void CreateValidTaxInformationAddress_Defaults_AreSet()
    {
        var entity = CreateValidTaxInformationAddress();

        Assert.NotNull(entity);
        Assert.Equal(1, entity.TaxInformationId);
        Assert.Equal(1, entity.AddressId);
        Assert.True(entity.IsPrimary);
        Assert.True(entity.IsCurrent);
        Assert.True(entity.ValidFrom <= DateTime.UtcNow);
    }

    [Fact]
    public void CreateValidTaxInformationAddress_CustomValues_AreApplied()
    {
        var date = new DateTime(2024, 1, 1);
        var entity = CreateValidTaxInformationAddress(
            taxInformationId: 2,
            addressId: 3,
            isPrimary: false,
            validFrom: date,
            validTo: date.AddDays(10),
            isCurrent: false);

        Assert.Equal(2, entity.TaxInformationId);
        Assert.Equal(3, entity.AddressId);
        Assert.False(entity.IsPrimary);
        Assert.False(entity.IsCurrent);
        Assert.Equal(date, entity.ValidFrom);
        Assert.Equal(date.AddDays(10), entity.ValidTo);
    }
}


