using EBOS.CRM.ApiTests.Fixtures.EntitiesFactories;
using EBOS.CRM.Domain.Entities;

namespace EBOS.CRM.ApiTests.Fixtures.EntitiesFactories;

public class StatusEntityFactoryTest
{
    public static Status CreateValidCountry(string description = "Active")
    {
        return new Status
        {
            Description = description
        };
    }
    [Fact]
    public void CreateValidCountry_Defaults_AreSet()
    {
        var status = CreateValidCountry();

        Assert.NotNull(status);
        Assert.Equal("Active", status.Description);
    }

    [Fact]
    public void CreateValidCountry_CustomValue_IsApplied()
    {
        var status = CreateValidCountry("Suspended");

        Assert.Equal("Suspended", status.Description);
    }
}
