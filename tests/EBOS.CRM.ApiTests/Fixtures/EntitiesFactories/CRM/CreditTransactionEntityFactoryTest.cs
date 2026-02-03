using EBOS.CRM.Domain.Entities.CRM;

namespace EBOS.CRM.ApiTests.Fixtures.EntitiesFactories.CRM;

public class CreditTransactionEntityFactoryTest
{
    public static CreditTransaction CreateValidCreditTransaction(
        DateTime? date = null,
        decimal amount = 10m,
        string type = "Consumption",
        string? externalReference = "REF-1",
        string? comments = "Test",
        long creditAccountId = 1)
    {
        return new CreditTransaction
        {
            Date = date ?? DateTime.UtcNow,
            Amount = amount,
            Type = type,
            ExternalReference = externalReference,
            Comments = comments,
            CreditAccountId = creditAccountId
        };
    }

    [Fact]
    public void CreateValidCreditTransaction_Defaults_AreSet()
    {
        var entity = CreateValidCreditTransaction();

        Assert.NotNull(entity);
        Assert.Equal(10m, entity.Amount);
        Assert.Equal("Consumption", entity.Type);
        Assert.Equal("REF-1", entity.ExternalReference);
        Assert.Equal("Test", entity.Comments);
        Assert.Equal(1, entity.CreditAccountId);
    }

    [Fact]
    public void CreateValidCreditTransaction_CustomValues_AreApplied()
    {
        var date = new DateTime(2024, 1, 1);
        var entity = CreateValidCreditTransaction(
            date: date,
            amount: 25m,
            type: "Adjustment",
            externalReference: "REF-2",
            comments: "Ok",
            creditAccountId: 3);

        Assert.Equal(date, entity.Date);
        Assert.Equal(25m, entity.Amount);
        Assert.Equal("Adjustment", entity.Type);
        Assert.Equal("REF-2", entity.ExternalReference);
        Assert.Equal("Ok", entity.Comments);
        Assert.Equal(3, entity.CreditAccountId);
    }
}


