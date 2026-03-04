using EBOS.CRM.Domain.Entities.CRM;

namespace EBOS.CRM.ApiTests.Fixtures.EntitiesFactories.CRM;

public class BankInformationEntityFactoryTest
{
    private static BankInformation CreateValidBankInformation(string iban = "ES1200000000000000000000",
        string? bic = "BANKESMM", string? bankName = "Bank", long customerId = 1)
    {
        return new BankInformation
        {
            Iban = iban,
            Bic = bic,
            BankName = bankName,
            CustomerId = customerId
        };
    }
    [Fact]
    public void CreateValidBankInformation_Defaults_AreSet()
    {
        var entity = CreateValidBankInformation();

        Assert.NotNull(entity);
        Assert.Equal("ES1200000000000000000000", entity.Iban);
        Assert.Equal("BANKESMM", entity.Bic);
        Assert.Equal("Bank", entity.BankName);
        Assert.Equal(1, entity.CustomerId);
    }

    [Fact]
    public void CreateValidBankInformation_CustomValues_AreApplied()
    {
        var entity = CreateValidBankInformation(
            iban: "ES9900000000000000000000",
            bic: "BICCODE",
            bankName: "Custom Bank",
            customerId: 2);

        Assert.Equal("ES9900000000000000000000", entity.Iban);
        Assert.Equal("BICCODE", entity.Bic);
        Assert.Equal("Custom Bank", entity.BankName);
        Assert.Equal(2, entity.CustomerId);
    }
}


