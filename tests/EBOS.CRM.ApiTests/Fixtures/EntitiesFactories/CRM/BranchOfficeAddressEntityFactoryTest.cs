using EBOS.CRM.Domain.Entities.CRM;

namespace EBOS.CRM.ApiTests.Fixtures.EntitiesFactories.CRM;

public class BranchOfficeAddressEntityFactoryTest
{
    private static BranchOfficeAddress CreateValidBranchOfficeAddress(long branchOfficeId = 1, long addressId = 1,
        bool isPrimary = true, DateTime? validFrom = null, DateTime? validTo = null, bool isCurrent = true)
    {
        return new BranchOfficeAddress
        {
            BranchOfficeId = branchOfficeId,
            AddressId = addressId,
            IsPrimary = isPrimary,
            ValidFrom = validFrom ?? DateTime.UtcNow.AddDays(-1),
            ValidTo = validTo,
            IsCurrent = isCurrent
        };
    }

    [Fact]
    public void CreateValidBranchOfficeAddress_Defaults_AreSet()
    {
        var entity = CreateValidBranchOfficeAddress();

        Assert.NotNull(entity);
        Assert.Equal(1, entity.BranchOfficeId);
        Assert.Equal(1, entity.AddressId);
        Assert.True(entity.IsPrimary);
        Assert.True(entity.IsCurrent);
        Assert.True(entity.ValidFrom <= DateTime.UtcNow);
    }

    [Fact]
    public void CreateValidBranchOfficeAddress_CustomValues_AreApplied()
    {
        var date = new DateTime(2024, 1, 1);
        var entity = CreateValidBranchOfficeAddress(
            branchOfficeId: 2,
            addressId: 3,
            isPrimary: false,
            validFrom: date,
            validTo: date.AddDays(10),
            isCurrent: false);

        Assert.Equal(2, entity.BranchOfficeId);
        Assert.Equal(3, entity.AddressId);
        Assert.False(entity.IsPrimary);
        Assert.False(entity.IsCurrent);
        Assert.Equal(date, entity.ValidFrom);
        Assert.Equal(date.AddDays(10), entity.ValidTo);
    }
}


