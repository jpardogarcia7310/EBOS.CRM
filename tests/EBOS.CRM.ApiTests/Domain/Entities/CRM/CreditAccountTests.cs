using EBOS.CRM.Domain.Entities.CRM;

namespace EBOS.CRM.ApiTests.Domain.Entities.CRM;

public class CreditAccountTests
{
    [Fact]
    public void AvailableAmount_IsComputedAsMaxMinusUsed()
    {
        var entity = new CreditAccount
        {
            MaxAmount = 1000m,
            UsedAmount = 250m
        };

        Assert.Equal(750m, entity.AvailableAmount);
    }

    [Fact]
    public void CreditTransactions_Collection_IsInitialized()
    {
        var entity = new CreditAccount();
        Assert.NotNull(entity.CreditTransactions);
        Assert.Empty(entity.CreditTransactions);
    }
}
