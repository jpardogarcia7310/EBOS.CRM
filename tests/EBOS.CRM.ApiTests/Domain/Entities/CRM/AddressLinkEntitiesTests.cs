using EBOS.CRM.Domain.Entities.CRM;

namespace EBOS.CRM.ApiTests.Domain.Entities.CRM;

public class AddressLinkEntitiesTests
{
    [Fact]
    public void CustomerAddress_CanStoreValidityWindow()
    {
        var from = DateTime.UtcNow;
        var to = from.AddDays(10);
        var entity = new CustomerAddress
        {
            TenantId = 1,
            CustomerId = 2,
            AddressId = 3,
            IsPrimary = true,
            ValidFrom = from,
            ValidTo = to,
            IsCurrent = true,
            CreatedAt = from,
            CreatedBy = 10
        };

        Assert.Equal(1, entity.TenantId);
        Assert.True(entity.IsPrimary);
        Assert.Equal(to, entity.ValidTo);
    }

    [Fact]
    public void BranchOfficeAddress_CanStoreValidityWindow()
    {
        var from = DateTime.UtcNow;
        var entity = new BranchOfficeAddress
        {
            TenantId = 1,
            BranchOfficeId = 2,
            AddressId = 3,
            IsPrimary = false,
            ValidFrom = from,
            ValidTo = null,
            IsCurrent = true
        };

        Assert.Equal(2, entity.BranchOfficeId);
        Assert.True(entity.IsCurrent);
    }

    [Fact]
    public void TaxInformationAddress_CanStoreValidityWindow()
    {
        var from = DateTime.UtcNow;
        var entity = new TaxInformationAddress
        {
            TenantId = 1,
            TaxInformationId = 2,
            AddressId = 3,
            IsPrimary = true,
            ValidFrom = from,
            IsCurrent = true
        };

        Assert.Equal(2, entity.TaxInformationId);
        Assert.True(entity.IsPrimary);
    }
}
