using EBOS.CRM.Domain.Entities.CRM;

namespace EBOS.CRM.ApiTests.Domain.Entities.CRM;

public class BranchOfficeTests
{
    [Fact]
    public void BranchOffice_Collections_AreInitialized()
    {
        var entity = new BranchOffice();
        Assert.NotNull(entity.BranchOfficeAddresses);
        Assert.Empty(entity.BranchOfficeAddresses);
    }
}
