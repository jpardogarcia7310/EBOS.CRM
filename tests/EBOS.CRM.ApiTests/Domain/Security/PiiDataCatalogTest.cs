using EBOS.CRM.Domain.Security;

namespace EBOS.CRM.ApiTests.Domain.Security;

public class PiiDataCatalogTest
{
    [Fact]
    public void Customer360DefaultFields_ContainsAllCatalogEntries()
    {
        var fields = PiiDataCatalog.Customer360DefaultFields;

        Assert.Contains(PiiDataCatalog.CustomerEmail, fields);
        Assert.Contains(PiiDataCatalog.CustomerPhone, fields);
        Assert.Contains(PiiDataCatalog.CorporateTaxIdentification, fields);
        Assert.Contains(PiiDataCatalog.IndividualIdentificationNumber, fields);
        Assert.Contains(PiiDataCatalog.IndividualFirstName, fields);
        Assert.Contains(PiiDataCatalog.IndividualLastName, fields);
        Assert.Equal(6, fields.Length);
    }
}
