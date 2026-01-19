using EBOS.CRM.Domain.Entities;
using EBOS.CRM.Infrastructure.Persistence;

namespace EBOS.CRM.ApiTests.Fixtures;

public static class IntegrationTestCountriesDataSeeder
{
    public static void Seed(CrmDbContext context)
    {
        if (context.Paises.Any())
            return;

        var countries = new List<Pais>
            {
                new() {
                    Name = "España",
                    Iso31661A2Code = "ES",
                    Iso31661A3Code = "ESP",
                    Iso31661NumCode = "724",
                    Domain = ".es",
                    Currency = "Euro",
                    CurrencyCode = "EUR",
                    InternationalPhoneCode = "34"
                },
                new() {
                    Name = "Estados Unidos",
                    Iso31661A2Code = "US",
                    Iso31661A3Code = "USA",
                    Iso31661NumCode = "840",
                    Domain = ".us",
                    Currency = "Dólar estadounidense",
                    CurrencyCode = "USD",
                    InternationalPhoneCode = "1"
                },
                new() {
                    Name = "Alemania",
                    Iso31661A2Code = "DE",
                    Iso31661A3Code = "DEU",
                    Iso31661NumCode = "276",
                    Domain = ".de",
                    Currency = "Euro",
                    CurrencyCode = "EUR",
                    InternationalPhoneCode = "49"
                }
            };

        context.Paises.AddRange(countries);
        context.SaveChanges();
    }
}