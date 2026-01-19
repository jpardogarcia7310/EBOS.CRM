using EBOS.CRM.Domain.Entities;

namespace EBOS.CRM.ApiTests.Fixtures.EntitiesFactories;

public static class CountryEntityFactory
{
    public static Pais CreateValidCountry(string name = "España", string iso2 = "ES", string iso3 = "ESP", string num = "724")
    {
        return new Pais
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
}