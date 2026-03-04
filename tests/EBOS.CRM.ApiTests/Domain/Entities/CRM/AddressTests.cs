using EBOS.CRM.Domain.Entities.CRM;

namespace EBOS.CRM.ApiTests.Domain.Entities.CRM;

public class AddressTests
{
    [Fact]
    public void Address_Collections_AreInitialized()
    {
        var entity = new Address();

        Assert.NotNull(entity.CustomerAddresses);
        Assert.NotNull(entity.BranchOfficeAddresses);
        Assert.NotNull(entity.TaxInformationAddresses);
        Assert.Empty(entity.CustomerAddresses);
        Assert.Empty(entity.BranchOfficeAddresses);
        Assert.Empty(entity.TaxInformationAddresses);
    }
}
