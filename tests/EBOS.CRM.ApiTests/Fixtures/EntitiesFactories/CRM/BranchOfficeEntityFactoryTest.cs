using EBOS.CRM.Domain.Entities.CRM;

namespace EBOS.CRM.ApiTests.Fixtures.EntitiesFactories.CRM;

public class BranchOfficeEntityFactoryTest
{
    public static BranchOffice CreateValidBranchOffice(
        string name = "Main",
        string phoneNumber = "123",
        long corporateCustomerId = 1)
    {
        return new BranchOffice
        {
            Name = name,
            PhoneNumber = phoneNumber,
            CorporateCustomerId = corporateCustomerId
        };
    }

    [Fact]
    public void CreateValidBranchOffice_Defaults_AreSet()
    {
        var entity = CreateValidBranchOffice();

        Assert.NotNull(entity);
        Assert.Equal("Main", entity.Name);
        Assert.Equal("123", entity.PhoneNumber);
        Assert.Equal(1, entity.CorporateCustomerId);
    }

    [Fact]
    public void CreateValidBranchOffice_CustomValues_AreApplied()
    {
        var entity = CreateValidBranchOffice(
            name: "HQ",
            phoneNumber: "999",
            corporateCustomerId: 2);

        Assert.Equal("HQ", entity.Name);
        Assert.Equal("999", entity.PhoneNumber);
        Assert.Equal(2, entity.CorporateCustomerId);
    }
}


