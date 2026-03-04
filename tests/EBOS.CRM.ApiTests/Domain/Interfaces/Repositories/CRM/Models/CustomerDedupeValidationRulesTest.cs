using EBOS.CRM.Domain.Interfaces.Repositories.CRM.Models;

namespace EBOS.CRM.ApiTests.Domain.Interfaces.Repositories.CRM.Models;

public class CustomerDedupeValidationRulesTest
{
    [Fact]
    public void Constants_AreExpected()
    {
        Assert.Equal(100, CustomerDedupeValidationRules.MaxEmailLength);
        Assert.Equal(12, CustomerDedupeValidationRules.MaxPhoneDigits);
        Assert.Equal(20, CustomerDedupeValidationRules.MaxTaxIdLength);
        Assert.Equal(10, CustomerDedupeValidationRules.MaxIdentificationNumberLength);
        Assert.Equal("^[A-Za-z0-9]+$", CustomerDedupeValidationRules.AlphanumericPattern);
        Assert.Equal("^\\d+$", CustomerDedupeValidationRules.DigitsPattern);
    }
}
