using EBOS.CRM.Domain.Entities;

namespace EBOS.CRM.ApiTests.Fixtures.EntitiesFactories.EBOS;

public class CountryEntityFactoryTest
{
    private static Country CreateValidCountry(string name = "España", string iso2 = "ES", string iso3 = "ESP",
        string num = "724")
    {
        return new Country
        {
            Name = name,
            Iso31661A2Code = iso2,
            Iso31661A3Code = iso3,
            Iso31661NumCode = num,
            Domain = ".es",
            Currency = "Euro",
            CurrencyCode = "EUR",
            InternationalPhoneCode = "34"
        };
    }
    [Fact]
    public void CreateValidCountry_Defaults_AreSet()
    {
        var country = CreateValidCountry();

        Assert.NotNull(country);
        Assert.Equal("España", country.Name);
        Assert.Equal("ES", country.Iso31661A2Code);
        Assert.Equal("ESP", country.Iso31661A3Code);
        Assert.Equal("724", country.Iso31661NumCode);
        Assert.Equal(".es", country.Domain);
        Assert.Equal("Euro", country.Currency);
        Assert.Equal("EUR", country.CurrencyCode);
        Assert.Equal("34", country.InternationalPhoneCode);
    }

    [Fact]
    public void CreateValidCountry_CustomValues_AreApplied()
    {
        var country = CreateValidCountry(
            name: "USA",
            iso2: "US",
            iso3: "USA",
            num: "840");

        Assert.Equal("USA", country.Name);
        Assert.Equal("US", country.Iso31661A2Code);
        Assert.Equal("USA", country.Iso31661A3Code);
        Assert.Equal("840", country.Iso31661NumCode);
    }
}



