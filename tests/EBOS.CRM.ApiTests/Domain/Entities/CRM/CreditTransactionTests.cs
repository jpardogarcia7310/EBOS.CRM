using System.ComponentModel.DataAnnotations;
using EBOS.CRM.Domain.Entities.CRM;

namespace EBOS.CRM.ApiTests.Domain.Entities.CRM;

public class CreditTransactionTests
{
    [Fact]
    public void ExternalReference_HasRequired_AndMaxLength200()
    {
        var prop = typeof(CreditTransaction).GetProperty(nameof(CreditTransaction.ExternalReference));
        Assert.NotNull(prop);
        Assert.NotNull(prop!.GetCustomAttributes(typeof(RequiredAttribute), true).SingleOrDefault());
        var max = prop.GetCustomAttributes(typeof(MaxLengthAttribute), true).Cast<MaxLengthAttribute>().SingleOrDefault();
        Assert.NotNull(max);
        Assert.Equal(200, max!.Length);
    }

    [Fact]
    public void Comments_HasRequired_AndMaxLength500()
    {
        var prop = typeof(CreditTransaction).GetProperty(nameof(CreditTransaction.Comments));
        Assert.NotNull(prop);
        Assert.NotNull(prop!.GetCustomAttributes(typeof(RequiredAttribute), true).SingleOrDefault());
        var max = prop.GetCustomAttributes(typeof(MaxLengthAttribute), true).Cast<MaxLengthAttribute>().SingleOrDefault();
        Assert.NotNull(max);
        Assert.Equal(500, max!.Length);
    }
}
