using EBOS.CRM.Domain.Entities.CRM;

namespace EBOS.CRM.ApiTests.Domain.Entities.CRM;

public class TaxInformationTests
{
    [Fact]
    public void TaxInformationAddresses_Collection_IsInitialized()
    {
        var entity = new TaxInformation();
        Assert.NotNull(entity.TaxInformationAddresses);
        Assert.Empty(entity.TaxInformationAddresses);
    }
}
