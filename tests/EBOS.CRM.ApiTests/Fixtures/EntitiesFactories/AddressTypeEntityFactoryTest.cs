using EBOS.CRM.ApiTests.Fixtures.EntitiesFactories;
using EBOS.CRM.Domain.Entities;

namespace EBOS.CRM.ApiTests.Fixtures.EntitiesFactories;

public class AddressTypeEntityFactoryTest
{
    public static AddressType CreateValidAddressType(string code = "HOME", string description = "Home")
    {
        return new AddressType
        {
            Code = code,
            Description = description,
            Category = "Shipping",
            AllowsMultiple = true,
            RequiresPrimary = false
        };
    }
    [Fact]
    public void CreateValidAddressType_Defaults_AreSet()
    {
        var addressType = CreateValidAddressType();

        Assert.NotNull(addressType);
        Assert.Equal("HOME", addressType.Code);
        Assert.Equal("Home", addressType.Description);
        Assert.Equal("Shipping", addressType.Category);
        Assert.True(addressType.AllowsMultiple);
        Assert.False(addressType.RequiresPrimary);
    }

    [Fact]
    public void CreateValidAddressType_CustomValues_AreApplied()
    {
        var addressType = CreateValidAddressType(code: "BILL", description: "Billing");

        Assert.Equal("BILL", addressType.Code);
        Assert.Equal("Billing", addressType.Description);
    }
}
