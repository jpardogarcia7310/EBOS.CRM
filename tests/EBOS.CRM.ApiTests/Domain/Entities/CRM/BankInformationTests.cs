using System.ComponentModel.DataAnnotations;
using EBOS.CRM.Domain.Entities.CRM;

namespace EBOS.CRM.ApiTests.Domain.Entities.CRM;

public class BankInformationTests
{
    [Fact]
    public void Iban_HasRequired_AndMaxLength34()
    {
        var iban = typeof(BankInformation).GetProperty(nameof(BankInformation.Iban));
        Assert.NotNull(iban);
        Assert.NotNull(iban!.GetCustomAttributes(typeof(RequiredAttribute), true).SingleOrDefault());

        var max = iban.GetCustomAttributes(typeof(MaxLengthAttribute), true).Cast<MaxLengthAttribute>().SingleOrDefault();
        Assert.NotNull(max);
        Assert.Equal(34, max!.Length);
    }

    [Fact]
    public void Bic_HasMaxLength11()
    {
        var bic = typeof(BankInformation).GetProperty(nameof(BankInformation.Bic));
        var max = bic!.GetCustomAttributes(typeof(MaxLengthAttribute), true).Cast<MaxLengthAttribute>().SingleOrDefault();
        Assert.NotNull(max);
        Assert.Equal(11, max!.Length);
    }
}
