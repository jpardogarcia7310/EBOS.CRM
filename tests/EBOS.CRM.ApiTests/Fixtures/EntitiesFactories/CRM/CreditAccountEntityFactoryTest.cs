using EBOS.CRM.ApiTests.Fixtures.EntitiesFactories.CRM;
using EBOS.CRM.Domain.Entities.CRM;

namespace EBOS.CRM.ApiTests.Fixtures.EntitiesFactories.CRM;

public class CreditAccountEntityFactoryTest
{
    public static CreditAccount CreateValidCreditAccount(
        decimal maxAmount = 1000m,
        decimal usedAmount = 100m,
        long customerId = 1)
    {
        return new CreditAccount
        {
            MaxAmount = maxAmount,
            UsedAmount = usedAmount,
            CustomerId = customerId
        };
    }

    [Fact]
    public void CreateValidCreditAccount_Defaults_AreSet()
    {
        var entity = CreateValidCreditAccount();

        Assert.NotNull(entity);
        Assert.Equal(1000m, entity.MaxAmount);
        Assert.Equal(100m, entity.UsedAmount);
        Assert.Equal(1, entity.CustomerId);
    }

    [Fact]
    public void CreateValidCreditAccount_CustomValues_AreApplied()
    {
        var entity = CreateValidCreditAccount(
            maxAmount: 5000m,
            usedAmount: 250m,
            customerId: 2);

        Assert.Equal(5000m, entity.MaxAmount);
        Assert.Equal(250m, entity.UsedAmount);
        Assert.Equal(2, entity.CustomerId);
    }
}
