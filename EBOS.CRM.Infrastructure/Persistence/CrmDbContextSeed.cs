using EBOS.CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EBOS.CRM.Infrastructure.Persistence;

public static class CrmDbContextSeed
{
    public static async Task SeedAsync(CrmDbContext context, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(context);

        // Seeding Countries: reduced and validated example.
        if (!await context.Countries.AnyAsync(cancellationToken))
        {
            await SeedPaises(context, cancellationToken);
        }
        if (!await context.Statuses.AnyAsync(cancellationToken))
        {
            await SeedEstados(context, cancellationToken);
        }
        if (!await context.AddressTypes.AnyAsync(cancellationToken))
        {
            await SeedAddressTypes(context, cancellationToken);
        }
        if (!await context.IdentificationTypes.AnyAsync(cancellationToken))
        {
            await SeedIdentificationTypes(context, cancellationToken);
        }
    }

    private static async Task SeedEstados(CrmDbContext context, CancellationToken cancellationToken)
    {
        var statuses = new List<Status>
        {
            new() {
                Description = "Activo" },
            new() {
                Description = "Moroso" },
            new() {
                Description = "Suspendido" }
        };
        // Basic validation before insertion
        var invalid = statuses
            .Select((s, i) => new { Index = i, Status = s })
            .Where(x =>
                string.IsNullOrWhiteSpace(x.Status.Description))
            .ToList();
        if (invalid.Count != 0)
        {
            throw new InvalidOperationException("Seed data contains invalid status entries. Please validate the seed source.");
        }

        await using var tx = await context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            await context.AddRangeAsync(statuses, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            await tx.CommitAsync(cancellationToken);
        }
        catch
        {
            await tx.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private static async Task SeedAddressTypes(CrmDbContext context, CancellationToken cancellationToken)
    {
        var addressTypes = new List<AddressType>
        {
            new() {
                Code = "DNI",
                Description = "Documento Nacional de Identidad" },
            new() {
                Code = "NIE",
                Description = "Numero de Identificación de Extranjeros" },
            new() {
                Code = "NIF",
                Description = "Numero de Identificación Fiscal" },
            new() {
                Code = "PASS",
                Description = "Pasaporte"
            }
        };
        // Basic validation before insertion
        var invalid = addressTypes
            .Select((s, i) => new { Index = i, AddressType = s })
            .Where(x =>
                string.IsNullOrWhiteSpace(x.AddressType.Description))
            .ToList();
        if (invalid.Count != 0)
        {
            throw new InvalidOperationException("Seed data contains invalid AddressTypes entries. " +
                                                "Please validate the seed source.");
        }

        await using var tx = await context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            await context.AddRangeAsync(addressTypes, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            await tx.CommitAsync(cancellationToken);
        }
        catch
        {
            await tx.RollbackAsync(cancellationToken);
            throw;
        }
    }
    private static async Task SeedIdentificationTypes(CrmDbContext context, CancellationToken cancellationToken)
    {
        var identificationTypes = new List<IdentificationType>
        {
            new() {
                Code = "DNI",
                Description = "Documento Nacional de Identidad" },
            new() {
                Code = "NIE",
                Description = "Numero de Identificación de Extranjeros" },
            new() {
                Code = "NIF",
                Description = "Numero de Identificación Fiscal" },
            new() {
                Code = "PASS",
                Description = "Pasaporte"
            }
        };
        // Basic validation before insertion
        var invalid = identificationTypes
            .Select((s, i) => new { Index = i, IdentificationType = s })
            .Where(x =>
                string.IsNullOrWhiteSpace(x.IdentificationType.Description))
            .ToList();
        if (invalid.Count != 0)
        {
            throw new InvalidOperationException("Seed data contains invalid IdentificationTypes entries. " +
                                                "Please validate the seed source.");
        }

        await using var tx = await context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            await context.AddRangeAsync(identificationTypes, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            await tx.CommitAsync(cancellationToken);
        }
        catch
        {
            await tx.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private static async Task SeedPaises(CrmDbContext context, CancellationToken cancellationToken)
    {
        const string dolarCaribeOriental = "Dólar del Caribe Oriental";
        const string dolarAustraliano = "Dólar australiano";
        const string francoAfricaOccidental = "Franco CFA de África Occidental";
        const string francoAfricaCentral = "Franco CFA de África Central";
        const string dolarEstadounidense = "Dólar estadounidense";
        const string dolarNeozelandes = "Dólar neozelandés";

        var countries = new List<Country>
        {
            new() {
                Name = "Afganistan",
                Iso31661A2Code = "AF", Iso31661A3Code = "AFG", Iso31661NumCode= "004", Domain= ".af", InternationalPhoneCode = "93",
                Currency= "Afgani afgano", CurrencyCode = "AFN" },
            new() {
                Name = "Albania",
                Iso31661A2Code = "AL", Iso31661A3Code = "ALB", Iso31661NumCode = "008", Domain = ".al", InternationalPhoneCode = "355",
                Currency = "Lek albanés", CurrencyCode = "ALL" },
            new() {
                Name = "Alemania",
                Iso31661A2Code = "DE", Iso31661A3Code = "DEU", Iso31661NumCode = "276", Domain = ".de", InternationalPhoneCode = "49",
                Currency = "Euro", CurrencyCode = "EUR" },
            new() {
                Name = "Andorra",
                Iso31661A2Code = "AD", Iso31661A3Code = "AND", Iso31661NumCode = "020", Domain = ".ad", InternationalPhoneCode = "376",
                Currency = "Euro", CurrencyCode = "EUR" },
            new() {
                Name = "Angola",
                Iso31661A2Code = "AO", Iso31661A3Code = "AGO", Iso31661NumCode = "024", Domain = ".ao", InternationalPhoneCode = "244",
                Currency = "Kwanza angoleño", CurrencyCode ="AOA" },
            new() {
                Name = "Anguila",
                Iso31661A2Code = "AI", Iso31661A3Code = "AIA", Iso31661NumCode = "660", Domain = ".ai", InternationalPhoneCode = "1-264",
                Currency = "Kwanza angoleño", CurrencyCode = "AOA" },
            new() {
                Name = "Antigua y Barbuda",
                Iso31661A2Code = "AG", Iso31661A3Code = "ATG", Iso31661NumCode = "028", Domain = ".ag", InternationalPhoneCode = "1-268",
                Currency = dolarCaribeOriental, CurrencyCode = "XCD" },
            new() {
                Name = "Arabia Saudita",
                Iso31661A2Code = "SA", Iso31661A3Code = "SAU", Iso31661NumCode = "682", Domain = ".sa", InternationalPhoneCode = "216",
                Currency = "Riyal saudí", CurrencyCode = "SAR" },
            new() {
                Name = "Argelia",
                Iso31661A2Code = "DZ", Iso31661A3Code = "DZA", Iso31661NumCode = "012", Domain = ".dz", InternationalPhoneCode = "213",
                Currency = "Dinar argelino", CurrencyCode = "DZD" },
            new() {
                Name = "Argentina",
                Iso31661A2Code = "AR", Iso31661A3Code = "ARG", Iso31661NumCode = "032", Domain = ".ar", InternationalPhoneCode = "54",
                Currency = "Peso", CurrencyCode = "ARS" },
            new() {
                Name = "Armenia",
                Iso31661A2Code = "AM", Iso31661A3Code = "ARM", Iso31661NumCode = "051", Domain = ".am", InternationalPhoneCode = "374",
                Currency = "Dram armenio", CurrencyCode = "AMD" },
            new() {
                Name = "Aruba",
                Iso31661A2Code = "AW", Iso31661A3Code = "ABW", Iso31661NumCode = "533", Domain = ".aw", InternationalPhoneCode = "297",
                Currency = "Kwanza angoleño", CurrencyCode = "AOA" },
            new() {
                Name = "Australia",
                Iso31661A2Code = "AU", Iso31661A3Code = "AUS", Iso31661NumCode = "036", Domain = ".au", InternationalPhoneCode = "61",
                Currency = dolarAustraliano, CurrencyCode = "AUD" },
            new() {
                Name = "Austria",
                Iso31661A2Code = "AT", Iso31661A3Code = "AUT", Iso31661NumCode = "040", Domain = ".at", InternationalPhoneCode = "43",
                Currency = "Euro", CurrencyCode = "EUR" },
            new() {
                Name = "Azerbaiyán",
                Iso31661A2Code = "AZ", Iso31661A3Code = "AZE", Iso31661NumCode = "031", Domain = ".az", InternationalPhoneCode = "994",
                Currency = "Manat azerí", CurrencyCode = "AZN" },

            new() {
                Name = "Bahamas",
                Iso31661A2Code = "BS", Iso31661A3Code = "BHS", Iso31661NumCode = "044", Domain = ".bs", InternationalPhoneCode = "1-242",
                Currency = "Dólar bahameño", CurrencyCode = "BSD" },
            new() {
                Name = "Bahrein",
                Iso31661A2Code = "BH", Iso31661A3Code = "BHR", Iso31661NumCode = "048", Domain = ".bh", InternationalPhoneCode = "973",
                Currency = "Dinar bahreiní", CurrencyCode = "BHD" },
            new() {
                Name = "Bailía de Guernsey",
                Iso31661A2Code = "GG", Iso31661A3Code = "GGY", Iso31661NumCode = "831", Domain = ".gg", InternationalPhoneCode = "44-1481",
                Currency = "Libra de Guernsey", CurrencyCode = "GGP" },
            new() {
                Name = "Bangladesh",
                Iso31661A2Code = "BD", Iso31661A3Code = "BGD", Iso31661NumCode = "050", Domain = ".bd", InternationalPhoneCode = "880",
                Currency = "Taka bangladeshí", CurrencyCode = "BDT" },
            new() {
                Name = "Barbados",
                Iso31661A2Code = "BB", Iso31661A3Code = "BRB", Iso31661NumCode = "052", Domain = ".bb", InternationalPhoneCode = "1-246",
                Currency = "Dólar de Barbados", CurrencyCode = "BBD" },
            new() {
                Name = "Belarús",
                Iso31661A2Code = "BY", Iso31661A3Code = "BLR", Iso31661NumCode = "112", Domain = ".by", InternationalPhoneCode = "375",
                Currency = "Rublo bielorruso", CurrencyCode = "BYN" },
            new() {
                Name = "Bélgica",
                Iso31661A2Code = "BE", Iso31661A3Code = "BEL", Iso31661NumCode = "056", Domain = ".be", InternationalPhoneCode = "32",
                Currency = "Euro", CurrencyCode = "EUR" },
            new() {
                Name = "Belice",
                Iso31661A2Code = "BZ", Iso31661A3Code = "BLZ", Iso31661NumCode = "084", Domain = ".bz" , InternationalPhoneCode = "501",
                Currency = "Dólar beliceño", CurrencyCode = "BZD" },
            new() {
                Name = "Benín",
                Iso31661A2Code = "BJ", Iso31661A3Code = "BEN", Iso31661NumCode = "204", Domain = ".bj", InternationalPhoneCode = "229",
                Currency = francoAfricaOccidental, CurrencyCode = "XOF" },
            new() {
                Name = "Bermudas",
                Iso31661A2Code = "BM", Iso31661A3Code = "BMU", Iso31661NumCode = "060", Domain = ".bm", InternationalPhoneCode = "1-441",
                Currency = "Bermudian dollar", CurrencyCode = "BMD" },
            new() {
                Name = "Bolivia",
                Iso31661A2Code = "BO", Iso31661A3Code = "BOL", Iso31661NumCode = "068", Domain = ".bo", InternationalPhoneCode = "591",
                Currency = "Boliviano", CurrencyCode = "BOB" },
            new() {
                Name = "Bosnia y Hercegovina",
                Iso31661A2Code = "BA", Iso31661A3Code = "BIH", Iso31661NumCode = "070", Domain = ".ba", InternationalPhoneCode = "387",
                Currency = "Marco convertible", CurrencyCode = "BAM" },
            new() {
                Name = "Botsuana",
                Iso31661A2Code = "BW", Iso31661A3Code = "BWA", Iso31661NumCode = "072", Domain = ".bw", InternationalPhoneCode = "267",
                Currency = "Pula", CurrencyCode = "BWP" },
            new() {
                Name = "Brasil",
                Iso31661A2Code = "BR", Iso31661A3Code = "BRA", Iso31661NumCode = "076", Domain = ".br", InternationalPhoneCode = "55",
                Currency = "Real brasileño", CurrencyCode = "BRL" },
            new() {
                Name = "Brunéi",
                Iso31661A2Code = "BN", Iso31661A3Code = "BRN", Iso31661NumCode = "096", Domain = ".bn", InternationalPhoneCode = "673",
                Currency = "Dólar de Brunéi", CurrencyCode = "BND" },
            new() {
                Name = "Bulgaria",
                Iso31661A2Code = "BG", Iso31661A3Code = "BGR", Iso31661NumCode = "100", Domain = ".bg", InternationalPhoneCode = "359",
                Currency = "Lev búlgaro", CurrencyCode = "BGN" },
            new() {
                Name = "Burkina Faso",
                Iso31661A2Code = "BF", Iso31661A3Code = "BFA", Iso31661NumCode = "854", Domain = ".bf", InternationalPhoneCode = "226",
                Currency = "ranco CFA de África Occidental", CurrencyCode = "XOF" },
            new() {
                Name = "Burundi",
                Iso31661A2Code = "BI", Iso31661A3Code = "BDI", Iso31661NumCode = "108", Domain = ".bi", InternationalPhoneCode = "257",
                Currency = "Franco de Burundi", CurrencyCode = "BIF" },
            new() {
                Name = "Bután",
                Iso31661A2Code = "BT", Iso31661A3Code = "BTN", Iso31661NumCode = "064", Domain = ".bt", InternationalPhoneCode = "975",
                Currency = "Ngultrum butanés",CurrencyCode = "BTN" },

            new() {
                Name = "Cabo Verde",
                Iso31661A2Code = "CV", Iso31661A3Code = "CPV", Iso31661NumCode = "132", Domain = ".cv", InternationalPhoneCode = "238",
                Currency = "Escudo caboverdiano", CurrencyCode = "CVE" },
            new() {
                Name = "Camboya",
                Iso31661A2Code = "KH", Iso31661A3Code = "KHM", Iso31661NumCode = "116", Domain = ".kh", InternationalPhoneCode = "855",
                Currency = "Riel camboyano", CurrencyCode = "KHR" },
            new() {
                Name = "Camerún",
                Iso31661A2Code = "CM", Iso31661A3Code = "CMR", Iso31661NumCode = "120", Domain = ".cm", InternationalPhoneCode = "237",
                Currency = francoAfricaCentral, CurrencyCode = "XAF" },
            new() {
                Name = "Canadá",
                Iso31661A2Code = "CA", Iso31661A3Code = "CAN", Iso31661NumCode = "124", Domain = ".ca", InternationalPhoneCode = "1",
                Currency = "Dólar canadiense", CurrencyCode = "CAD" },
            new() {
                Name = "Caribe Neerlandés",
                Iso31661A2Code = "BQ", Iso31661A3Code = "BES", Iso31661NumCode = "535", Domain = ".bq", InternationalPhoneCode = "599",
                Currency = dolarEstadounidense, CurrencyCode = "USD" },
            new() {
                Name = "Catar",
                Iso31661A2Code = "QA", Iso31661A3Code = "QAT", Iso31661NumCode = "634", Domain = ".qa", InternationalPhoneCode = "974",
                Currency = "Rial catarí", CurrencyCode = "QAR" },
            new() {
                Name = "Chad",
                Iso31661A2Code = "TD", Iso31661A3Code = "TCD", Iso31661NumCode = "148", Domain = ".td", InternationalPhoneCode = "235",
                Currency = francoAfricaCentral, CurrencyCode = "XAF" },
            new() {
                Name = "Chile",
                Iso31661A2Code = "CL", Iso31661A3Code = "CHL", Iso31661NumCode = "152", Domain = ".cl", InternationalPhoneCode = "56",
                Currency = "Peso chileno", CurrencyCode = "CLP" },
            new() {
                Name = "China",
                Iso31661A2Code = "CN", Iso31661A3Code = "CHN", Iso31661NumCode = "156", Domain = ".cn", InternationalPhoneCode = "86",
                Currency = "Yuan renminbi", CurrencyCode = "CNY" },
            new() {
                Name = "Chipre",
                Iso31661A2Code = "CY", Iso31661A3Code = "CYP", Iso31661NumCode = "196", Domain = ".cy", InternationalPhoneCode = "357",
                Currency = "Euro", CurrencyCode = "EUR" },
            new() {
                Name = "Ciudad del Vaticano",
                Iso31661A2Code = "VA", Iso31661A3Code = "VAT", Iso31661NumCode = "336", Domain = ".va", InternationalPhoneCode = "39-6",
                Currency = "Euro", CurrencyCode = "EUR" },
            new() {
                Name = "Colombia",
                Iso31661A2Code = "CO", Iso31661A3Code = "COL", Iso31661NumCode = "170", Domain = ".co", InternationalPhoneCode = "57",
                Currency = "Peso colombiano", CurrencyCode = "COP" },
            new() {
                Name = "Comores",
                Iso31661A2Code = "KM", Iso31661A3Code = "COM", Iso31661NumCode = "174", Domain = ".km", InternationalPhoneCode = "269",
                Currency = "Franco comorense", CurrencyCode = "KMF" },
            new() {
                Name = "Corea del Norte",
                Iso31661A2Code = "KP", Iso31661A3Code = "PRK", Iso31661NumCode = "408", Domain = ".kp", InternationalPhoneCode = "850",
                Currency = "Won norcoreano", CurrencyCode = "KPW" },
            new() {
                Name = "Corea del Sur",
                Iso31661A2Code = "KR", Iso31661A3Code = "KOR", Iso31661NumCode = "410", Domain = ".kr", InternationalPhoneCode = "82",
                Currency = "Won surcoreano", CurrencyCode = "KRW" },
            new() {
                Name = "Costa de Marfil",
                Iso31661A2Code = "CI", Iso31661A3Code = "CIV", Iso31661NumCode = "384", Domain = ".ci", InternationalPhoneCode = "225",
                Currency = francoAfricaOccidental, CurrencyCode = "XOF" },
            new() {
                Name = "Costa Rica",
                Iso31661A2Code = "CR", Iso31661A3Code = "CRI", Iso31661NumCode = "188", Domain = ".cr", InternationalPhoneCode = "506",
                Currency = "Colón costarricense", CurrencyCode = "CRC" },
            new() {
                Name = "Croacia",
                Iso31661A2Code = "HR", Iso31661A3Code = "HRV", Iso31661NumCode = "191", Domain = ".hr", InternationalPhoneCode = "385",
                Currency = "Kuna croata", CurrencyCode = "HRK" },
            new() {
                Name = "Cuba",
                Iso31661A2Code = "CU", Iso31661A3Code = "CUB", Iso31661NumCode = "192", Domain = ".cu", InternationalPhoneCode = "53",
                Currency = "Peso cubano", CurrencyCode = "CUP" },
            new() {
                Name = "Curaçao",
                Iso31661A2Code = "CW", Iso31661A3Code = "CUW", Iso31661NumCode = "531", Domain = ".cw", InternationalPhoneCode = "599",
                Currency = "Florín antillano neerlandés", CurrencyCode = "ANG" },

            new() {
                Name = "Dinamarca",
                Iso31661A2Code = "DK", Iso31661A3Code = "DNK", Iso31661NumCode = "208", Domain = ".dk", InternationalPhoneCode = "45",
                Currency = "Corona danesa", CurrencyCode = "DKK" },
            new() {
                Name = "Dominica",
                Iso31661A2Code = "DM", Iso31661A3Code = "DMA", Iso31661NumCode = "212", Domain = ".dm", InternationalPhoneCode = "1-767",
                Currency = dolarCaribeOriental, CurrencyCode = "XCD" },

            new() {
                Name = "Ecuador",
                Iso31661A2Code = "EC", Iso31661A3Code = "ECU", Iso31661NumCode = "218", Domain = ".ec", InternationalPhoneCode = "593",
                Currency = dolarEstadounidense, CurrencyCode = "USD" },
            new() {
                Name = "Egipto",
                Iso31661A2Code = "EG", Iso31661A3Code = "EGY", Iso31661NumCode = "818", Domain = ".eg", InternationalPhoneCode = "20",
                Currency = "Libra egipcia", CurrencyCode = "EGP" },
            new() {
                Name = "El Salvador",
                Iso31661A2Code = "SV", Iso31661A3Code = "SLV", Iso31661NumCode = "222", Domain = ".sv", InternationalPhoneCode = "503",
                Currency = dolarEstadounidense, CurrencyCode = "USD" },
            new() {
                Name = "Emiratos Árabes Unidos",
                Iso31661A2Code = "AE", Iso31661A3Code = "ARE", Iso31661NumCode = "784", Domain = ".ae", InternationalPhoneCode = "971",
                Currency = "Dirham de los Emiratos Árabes Unidos", CurrencyCode = "AED" },
            new() {
                Name = "Eritrea",
                Iso31661A2Code = "ER", Iso31661A3Code = "ERI", Iso31661NumCode = "232", Domain = ".er", InternationalPhoneCode = "291",
                Currency = "Nakfa eritreo", CurrencyCode = "ERN" },
            new() {
                Name = "Eslovaquia",
                Iso31661A2Code = "SK", Iso31661A3Code = "SVK", Iso31661NumCode = "703", Domain = ".sk", InternationalPhoneCode = "421",
                Currency = "Euro", CurrencyCode = "EUR" },
            new() {
                Name = "Eslovenia",
                Iso31661A2Code = "SI", Iso31661A3Code = "SVN", Iso31661NumCode = "705", Domain = ".si", InternationalPhoneCode = "386",
                Currency = "Euro", CurrencyCode = "EUR" },
            new() {
                Name = "España",
                Iso31661A2Code = "ES", Iso31661A3Code = "ESP", Iso31661NumCode = "724", Domain = ".es", InternationalPhoneCode = "34",
                Currency = "Euro", CurrencyCode = "EUR" },
            new() {
                Name = "Estados Federados de Micronesia",
                Iso31661A2Code = "FM", Iso31661A3Code = "FSM", Iso31661NumCode = "583", Domain = ".fm", InternationalPhoneCode = "691",
                Currency = dolarEstadounidense, CurrencyCode = "USD" },
            new() {
                Name = "Estados Unidos",
                Iso31661A2Code = "US", Iso31661A3Code = "USA", Iso31661NumCode = "840", Domain = ".us", InternationalPhoneCode = "1",
                Currency = dolarEstadounidense, CurrencyCode = "USD" },
            new() {
                Name = "Estonia",
                Iso31661A2Code = "EE", Iso31661A3Code = "EST", Iso31661NumCode = "233", Domain = ".ee", InternationalPhoneCode = "372",
                Currency = "Euro", CurrencyCode = "EUR" },
            new() {
                Name = "Esuatini",
                Iso31661A2Code = "SZ", Iso31661A3Code = "SWZ", Iso31661NumCode = "748", Domain = ".sz", InternationalPhoneCode = "268",
                Currency = "Lilangeni suazi", CurrencyCode = "SZL" },
            new() {
                Name = "Etiopía",
                Iso31661A2Code = "ET", Iso31661A3Code = "ETH", Iso31661NumCode = "231", Domain = ".et", InternationalPhoneCode = "251",
                Currency = "Birr etíope", CurrencyCode = "ETB" },

            new() {
                Name = "Filipinas",
                Iso31661A2Code = "PH", Iso31661A3Code = "PHL", Iso31661NumCode = "608", Domain = ".ph", InternationalPhoneCode = "63",
                Currency = "Peso filipino", CurrencyCode = "PHP" },
            new() {
                Name = "Finlandia",
                Iso31661A2Code = "FI", Iso31661A3Code = "FIN", Iso31661NumCode = "246", Domain = ".fi", InternationalPhoneCode = "358",
                Currency = "Euro", CurrencyCode = "EUR" },
            new() {
                Name = "Fiyi",
                Iso31661A2Code = "FJ", Iso31661A3Code = "FJI", Iso31661NumCode = "242", Domain = ".fj", InternationalPhoneCode = "679",
                Currency = "Dólar fiyiano", CurrencyCode = "FJD" },
            new() {
                Name = "Francia",
                Iso31661A2Code = "FR", Iso31661A3Code = "FRA", Iso31661NumCode = "250", Domain = ".fr", InternationalPhoneCode = "33",
                Currency = "Euro", CurrencyCode = "EUR" },

            new() {
                Name = "Gabón",
                Iso31661A2Code = "GA", Iso31661A3Code = "GAB", Iso31661NumCode = "266", Domain = ".ga", InternationalPhoneCode = "241",
                Currency = francoAfricaCentral, CurrencyCode = "XAF" },
            new() {
                Name = "Gambia",
                Iso31661A2Code = "GM", Iso31661A3Code = "GMB", Iso31661NumCode = "270", Domain = ".gm", InternationalPhoneCode = "220",
                Currency = "Dalasi gambiano", CurrencyCode = "GMD" },
            new() {
                Name = "Georgia",
                Iso31661A2Code = "GE", Iso31661A3Code = "GEO", Iso31661NumCode = "268", Domain = ".ge", InternationalPhoneCode = "995",
                Currency = "Lari georgiano", CurrencyCode = "GEL" },
            new() {
                Name = "Georgia del Sur y las Islas Sandwich del Sur\r\nTerritorio británico de ultramar",
                Iso31661A2Code = "GS", Iso31661A3Code = "SGS", Iso31661NumCode = "239", Domain = ".gs", InternationalPhoneCode = "500",
                Currency = "Libra esterlina", CurrencyCode = "GBP" },
            new() {
                Name = "Ghana",
                Iso31661A2Code = "GH", Iso31661A3Code = "GHA", Iso31661NumCode = "288", Domain = ".gh", InternationalPhoneCode = "233",
                Currency = "Cedi ghanés", CurrencyCode = "GHS" },
            new() {
                Name = "Gibraltar",
                Iso31661A2Code = "GI", Iso31661A3Code = "GIB", Iso31661NumCode = "292", Domain = ".gi", InternationalPhoneCode = "350",
                Currency = "Libra de Gibraltar", CurrencyCode = "GIP" },
            new() {
                Name = "Granada",
                Iso31661A2Code = "GD", Iso31661A3Code = "GRD", Iso31661NumCode = "308", Domain = ".gd", InternationalPhoneCode = "1-473",
                Currency = dolarCaribeOriental, CurrencyCode = "XCD" },
            new() {
                Name = "Grecia",
                Iso31661A2Code = "GR", Iso31661A3Code = "GRC", Iso31661NumCode = "300", Domain = ".gr", InternationalPhoneCode = "30",
                Currency = "Euro", CurrencyCode = "EUR" },
            new() {
                Name = "Groenlandia",
                Iso31661A2Code = "GL", Iso31661A3Code = "GRL", Iso31661NumCode = "304", Domain = ".gl", InternationalPhoneCode = "299",
                Currency = "Corona danesa", CurrencyCode = "DKK" },
            new() {
                Name = "Guadalupe",
                Iso31661A2Code = "GP", Iso31661A3Code = "GLP", Iso31661NumCode = "312", Domain = ".gp", InternationalPhoneCode = "590",
                Currency = "Euro", CurrencyCode = "EUR" },
            new() {
                Name = "Guam",
                Iso31661A2Code = "GU", Iso31661A3Code = "GUM", Iso31661NumCode = "316", Domain = ".gu", InternationalPhoneCode = "1-671",
                Currency = dolarEstadounidense, CurrencyCode = "USD" },
            new() {
                Name = "Guatemala",
                Iso31661A2Code = "GT", Iso31661A3Code = "GTM", Iso31661NumCode = "320", Domain = ".gt", InternationalPhoneCode = "502",
                Currency = "Quetzal guatemalteco", CurrencyCode = "GTQ" },
            new() {
                Name = "Guayana",
                Iso31661A2Code = "GY", Iso31661A3Code = "GUY", Iso31661NumCode = "328", Domain = ".gy", InternationalPhoneCode = "592",
                Currency = "Dólar guyanés", CurrencyCode = "GYD" },
            new() {
                Name = "Guayana Francesa",
                Iso31661A2Code = "GF", Iso31661A3Code = "GUF", Iso31661NumCode = "254", Domain = ".gf", InternationalPhoneCode = "594",
                Currency = "Euro", CurrencyCode = "EUR" },
            new() {
                Name = "Guinea",
                Iso31661A2Code = "GN", Iso31661A3Code = "GIN", Iso31661NumCode = "324", Domain = ".gn", InternationalPhoneCode = "224",
                Currency = "Franco guineano", CurrencyCode = "GNF" },
            new() {
                Name = "Guinea Ecuatorial",
                Iso31661A2Code = "GQ", Iso31661A3Code = "GNQ", Iso31661NumCode = "226", Domain = ".gq", InternationalPhoneCode = "240",
                Currency = francoAfricaCentral, CurrencyCode = "XAF" },
            new() {
                Name = "Guinea-Bisáu",
                Iso31661A2Code = "GW", Iso31661A3Code = "GNB", Iso31661NumCode = "624", Domain = ".gw", InternationalPhoneCode = "245",
                Currency = francoAfricaOccidental, CurrencyCode = "XOF" },

            new() {
                Name = "Haití",
                Iso31661A2Code = "HT", Iso31661A3Code = "HTI", Iso31661NumCode = "332", Domain = ".ht", InternationalPhoneCode = "509",
                Currency = "Gourde haitiano", CurrencyCode = "HTG" },
            new() {
                Name = "Honduras",
                Iso31661A2Code = "HN", Iso31661A3Code = "HND", Iso31661NumCode = "340", Domain = ".hn", InternationalPhoneCode = "504",
                Currency = "Lempira hondureño", CurrencyCode = "HNL" },
            new() {
                Name = "Hong Kong",
                Iso31661A2Code = "HK", Iso31661A3Code = "HKG", Iso31661NumCode = "344", Domain = ".hk", InternationalPhoneCode = "852",
                Currency = "Dólar de Hong Kong", CurrencyCode = "HKD" },
            new() {
                Name = "Hungría",
                Iso31661A2Code = "HU", Iso31661A3Code = "HUN", Iso31661NumCode = "348", Domain = ".hu", InternationalPhoneCode = "36",
                Currency = "Forinto húngaro", CurrencyCode = "HUF" },

            new() {
                Name = "India",
                Iso31661A2Code = "IN", Iso31661A3Code = "IND", Iso31661NumCode = "356", Domain = ".in", InternationalPhoneCode = "91",
                Currency = "Rupia india", CurrencyCode = "INR" },
            new() {
                Name = "Indonesia",
                Iso31661A2Code = "ID", Iso31661A3Code = "IDN", Iso31661NumCode = "360", Domain = ".id", InternationalPhoneCode = "62",
                Currency = "Rupia indonesia", CurrencyCode = "IDR" },
            new() {
                Name = "Irán",
                Iso31661A2Code = "IR", Iso31661A3Code = "IRN", Iso31661NumCode = "364", Domain = ".ir", InternationalPhoneCode = "98",
                Currency = "Rial iraní", CurrencyCode = "IRR" },
            new() {
                Name = "Iraq",
                Iso31661A2Code = "IQ", Iso31661A3Code = "IRQ", Iso31661NumCode = "368", Domain = ".iq", InternationalPhoneCode = "964",
                Currency = "Dinar iraquí", CurrencyCode = "IQD" },
            new() {
                Name = "Irlanda",
                Iso31661A2Code = "IE", Iso31661A3Code = "IRL", Iso31661NumCode = "372", Domain = ".ie", InternationalPhoneCode = "353",
                Currency = "Euro", CurrencyCode = "EUR" },
            new() {
                Name = "Isla Bouvet",
                Iso31661A2Code = "BV", Iso31661A3Code = "BVT", Iso31661NumCode = "074", Domain = ".bv", InternationalPhoneCode = "47",
                Currency = "Corona noruega", CurrencyCode = "NOK" },
            new() {
                Name = "Isla de Man",
                Iso31661A2Code = "IM", Iso31661A3Code = "IMN", Iso31661NumCode = "833", Domain = ".im", InternationalPhoneCode = "44-1624",
                Currency = "Libra Manesa", CurrencyCode = "IMP" },
            new() {
                Name = "Isla de Navidad",
                Iso31661A2Code = "CX", Iso31661A3Code = "CXR", Iso31661NumCode = "162", Domain = ".cx", InternationalPhoneCode = "61",
                Currency = dolarAustraliano, CurrencyCode = "AUD" },
            new() {
                Name = "Isla de San Martín",
                Iso31661A2Code = "MF", Iso31661A3Code = "MAF", Iso31661NumCode = "663", Domain = ".mf" , InternationalPhoneCode = "590",
                Currency = "Euro", CurrencyCode = "EUR" },
            new() {
                Name = "Isla Mauricio",
                Iso31661A2Code = "MU", Iso31661A3Code = "MUS", Iso31661NumCode = "480", Domain = ".mu", InternationalPhoneCode = "230",
                Currency = "Rupia mauriciana", CurrencyCode = "MUR" },
            new() {
                Name = "Isla Norfolk",
                Iso31661A2Code = "NF", Iso31661A3Code = "NFK", Iso31661NumCode = "574", Domain = ".nf", InternationalPhoneCode = "6723",
                Currency = dolarAustraliano, CurrencyCode = "AUD" },
            new() {
                Name = "Islandia",
                Iso31661A2Code = "IS", Iso31661A3Code = "ISL", Iso31661NumCode = "352", Domain = ".is", InternationalPhoneCode = "354",
                Currency = "Corona islandesa", CurrencyCode = "ISK" },
            new() {
                Name = "Islas Åland",
                Iso31661A2Code = "AX", Iso31661A3Code = "ALA", Iso31661NumCode = "248", Domain = ".ax", InternationalPhoneCode = "358",
                Currency = "Euro", CurrencyCode = "EUR" },
            new() {
                Name = "Islas Caimán",
                Iso31661A2Code = "KY", Iso31661A3Code = "CYM", Iso31661NumCode = "136", Domain = ".ky", InternationalPhoneCode = "1-345",
                Currency = "Dólar de las Islas Caimán", CurrencyCode = "KYD" },
            new() {
                Name = "Islas Cocos",
                Iso31661A2Code = "CC", Iso31661A3Code = "CCK", Iso31661NumCode = "166", Domain = ".cc", InternationalPhoneCode = "61-891",
                Currency = dolarAustraliano, CurrencyCode = "AUD" },
            new() {
                Name = "Islas Cook",
                Iso31661A2Code = "CK", Iso31661A3Code = "COK", Iso31661NumCode = "184", Domain = ".ck", InternationalPhoneCode = "682",
                Currency = dolarNeozelandes, CurrencyCode = "NZD" },
            new() {
                Name = "Islas Feroe",
                Iso31661A2Code = "FO", Iso31661A3Code = "FRO", Iso31661NumCode = "234", Domain = ".fo", InternationalPhoneCode = "298",
                Currency = "Corona danesa", CurrencyCode = "DKK" },
            new() {
                Name = "Islas Heard y McDonald",
                Iso31661A2Code = "HM", Iso31661A3Code = "HMD", Iso31661NumCode = "334", Domain = ".hm", InternationalPhoneCode = "0",
                Currency = dolarAustraliano, CurrencyCode = "AUD" },
            new() {
                Name = "Islas Malvinas",
                Iso31661A2Code = "FK", Iso31661A3Code = "FLK", Iso31661NumCode = "238", Domain = ".fk", InternationalPhoneCode = "500",
                Currency = "Libra malvinense", CurrencyCode = "FKP" },
            new() {
                Name = "Islas Marianas del Norte",
                Iso31661A2Code = "MP", Iso31661A3Code = "MNP", Iso31661NumCode = "580", Domain = ".mp", InternationalPhoneCode = "1-670",
                Currency = dolarEstadounidense, CurrencyCode = "USD" },
            new() {
                Name = "Islas Marshall",
                Iso31661A2Code = "MH", Iso31661A3Code = "MHL", Iso31661NumCode = "584", Domain = ".mh", InternationalPhoneCode = "692",
                Currency = dolarEstadounidense, CurrencyCode = "USD" },
            new() {
                Name = "Islas Pitcairn",
                Iso31661A2Code = "PN", Iso31661A3Code = "PCN", Iso31661NumCode = "612", Domain = ".pn", InternationalPhoneCode = "649",
                Currency = dolarNeozelandes, CurrencyCode = "NZD" },
            new() {
                Name = "Islas Salomón",
                Iso31661A2Code = "SB", Iso31661A3Code = "SLB", Iso31661NumCode = "090", Domain = ".sb", InternationalPhoneCode = "677",
                Currency = "Dólar de las Islas Salomón", CurrencyCode = "SBD" },
            new() {
                Name = "Islas Turcas y Caicos",
                Iso31661A2Code = "TC", Iso31661A3Code = "TCA", Iso31661NumCode = "796", Domain = ".tc", InternationalPhoneCode = "1-649",
                Currency = dolarCaribeOriental, CurrencyCode = "XCD" },
            new() {
                Name = "Islas ultramarinas menores de los Estados Unidos",
                Iso31661A2Code = "UM", Iso31661A3Code = "UMI", Iso31661NumCode = "581", Domain = ".us", InternationalPhoneCode = "1",
                Currency = dolarEstadounidense, CurrencyCode = "USD" },
            new() {
                Name = "Islas Vírgenes Británicas",
                Iso31661A2Code = "VG", Iso31661A3Code = "VGB", Iso31661NumCode = "092", Domain = ".vg", InternationalPhoneCode = "1-284",
                Currency = dolarEstadounidense, CurrencyCode = "USD" },
            new() {
                Name = "Islas Vírgenes de los Estados Unidos",
                Iso31661A2Code = "VI", Iso31661A3Code = "VIR", Iso31661NumCode = "850", Domain = ".vi", InternationalPhoneCode = "1-340",
                Currency = dolarEstadounidense, CurrencyCode = "USD" },
            new() {
                Name = "Israel",
                Iso31661A2Code = "IL", Iso31661A3Code = "ISR", Iso31661NumCode = "376", Domain = ".il", InternationalPhoneCode = "972",
                Currency = "Nuevo séquel israelí", CurrencyCode = "ILS" },
            new() {
                Name = "Italia",
                Iso31661A2Code = "IT", Iso31661A3Code = "ITA", Iso31661NumCode = "380", Domain = ".it", InternationalPhoneCode = "39",
                Currency = "Euro", CurrencyCode = "EUR" },

            new() { 
                Name = "Jamaica",
                Iso31661A2Code = "JM", Iso31661A3Code = "JAM", Iso31661NumCode = "388", Domain = ".jm", InternationalPhoneCode = "1-876",
                Currency = "Dólar jamaicano", CurrencyCode = "JMD" },
            new() { 
                Name = "Japón",
                Iso31661A2Code = "JP", Iso31661A3Code = "JPN", Iso31661NumCode = "392", Domain = ".jp", InternationalPhoneCode = "81",
                Currency = "Yen japonés", CurrencyCode = "JPY" },
            new() { 
                Name = "Jersey",
                Iso31661A2Code = "JE", Iso31661A3Code = "JEY", Iso31661NumCode = "832", Domain = ".je", InternationalPhoneCode = "44-1534",
                Currency = "Libra esterlina", CurrencyCode = "GBP" },
            new() {
                Name = "Jordania",
                Iso31661A2Code = "JO", Iso31661A3Code = "JOR", Iso31661NumCode = "400", Domain = ".jo", InternationalPhoneCode = "962",
                Currency = "Dinar jordano", CurrencyCode = "JOD" },

            new() {
                Name = "Kazajistán",
                Iso31661A2Code = "KZ", Iso31661A3Code = "KAZ", Iso31661NumCode = "398", Domain = ".kz", InternationalPhoneCode = "997",
                Currency = "Tenge kazajo", CurrencyCode = "KZT" },
            new() {
                Name = "Kenia",
                Iso31661A2Code = "KE", Iso31661A3Code = "KEN", Iso31661NumCode = "404", Domain = ".ke", InternationalPhoneCode = "254",
                Currency = "Chelín keniano", CurrencyCode = "KES" },
            new() {
                Name = "Kirguistán",
                Iso31661A2Code = "KG", Iso31661A3Code = "KGZ", Iso31661NumCode = "417", Domain = ".kg", InternationalPhoneCode = "996",
                Currency = "Som kirguís", CurrencyCode = "KGS" },
            new() {
                Name = "Kiribati",
                Iso31661A2Code = "KI", Iso31661A3Code = "KIR", Iso31661NumCode = "296", Domain = ".ki", InternationalPhoneCode = "686",
                Currency = dolarAustraliano, CurrencyCode = "AUD" },
            new() {
                Name = "Kosovo",
                Iso31661A2Code = "XK", Iso31661A3Code = "XXK", Iso31661NumCode = "412", Domain = ".ko", InternationalPhoneCode = "383",
                Currency = "Euro", CurrencyCode = "EUR" },
            new() {
                Name = "Kuwait",
                Iso31661A2Code = "KW", Iso31661A3Code = "KWT", Iso31661NumCode = "414", Domain = ".kw", InternationalPhoneCode = "965",
                Currency = "Dinar kuwaití", CurrencyCode = "KWD" },

            new() {
                Name = "Laos",
                Iso31661A2Code = "LA", Iso31661A3Code = "LAO", Iso31661NumCode = "418", Domain = ".la", InternationalPhoneCode = "865",
                Currency = "Kip laosiano", CurrencyCode = "LAK" },
            new() {
                Name = "Lesoto",
                Iso31661A2Code = "LS", Iso31661A3Code = "LSO", Iso31661NumCode = "426", Domain = ".ls", InternationalPhoneCode = "266",
                Currency = "Loti lesotense", CurrencyCode = "LSL" },
            new() {
                Name = "Letonia",
                Iso31661A2Code = "LV", Iso31661A3Code = "LVA", Iso31661NumCode = "428", Domain = ".lv", InternationalPhoneCode = "371",
                Currency = "Euro", CurrencyCode = "EUR" },
            new() {
                Name = "Líbano",
                Iso31661A2Code = "LB", Iso31661A3Code = "LBN", Iso31661NumCode = "422", Domain = ".lb", InternationalPhoneCode = "961",
                Currency = "Libra libanesa", CurrencyCode = "LBP" },
            new() {
                Name = "Liberia",
                Iso31661A2Code = "LR", Iso31661A3Code = "LBR", Iso31661NumCode = "430", Domain = ".lr", InternationalPhoneCode = "231",
                Currency = "Dólar liberiano", CurrencyCode = "LRD" },
            new() {
                Name = "Libia",
                Iso31661A2Code = "LY", Iso31661A3Code = "LBY", Iso31661NumCode = "434", Domain = ".ly", InternationalPhoneCode = "218",
                Currency = "Dinar libio", CurrencyCode = "LYD" },
            new() {
                Name = "Liechtenstein",
                Iso31661A2Code = "LI", Iso31661A3Code = "LIE", Iso31661NumCode = "438", Domain = ".li", InternationalPhoneCode = "423",
                Currency = "Franco suizo", CurrencyCode = "CHF" },
            new() {
                Name = "Lituania",
                Iso31661A2Code = "LT", Iso31661A3Code = "LTU", Iso31661NumCode = "440", Domain = ".lt", InternationalPhoneCode = "370",
                Currency = "Euro", CurrencyCode = "EUR" },
            new() {
                Name = "Luxemburgo",
                Iso31661A2Code = "LU", Iso31661A3Code = "LUX", Iso31661NumCode = "442", Domain = ".lu", InternationalPhoneCode = "352",
                Currency = "Euro", CurrencyCode = "EUR" },

            new() {
                Name = "Macao",
                Iso31661A2Code = "MO", Iso31661A3Code = "MAC", Iso31661NumCode = "446", Domain = ".mo", InternationalPhoneCode = "853",
                Currency = "Pataca de Macao", CurrencyCode = "MOP" },
            new() {
                Name = "Macedonia del Norte",
                Iso31661A2Code = "MK", Iso31661A3Code = "MKD", Iso31661NumCode = "807", Domain = ".mk", InternationalPhoneCode = "289",
                Currency = "Denar macedonio", CurrencyCode = "MKD" },
            new() {
                Name = "Madagascar",
                Iso31661A2Code = "MG", Iso31661A3Code = "MDG", Iso31661NumCode = "450", Domain = ".mg", InternationalPhoneCode = "261",
                Currency = "Ariary malgache", CurrencyCode = "MGA" },
            new() {
                Name = "Malasia",
                Iso31661A2Code = "MY", Iso31661A3Code = "MYS", Iso31661NumCode = "458", Domain = ".my", InternationalPhoneCode = "60",
                Currency = "Ringgit malasio", CurrencyCode = "MYR" },
            new() {
                Name = "Malawi",
                Iso31661A2Code = "MW", Iso31661A3Code = "MWI", Iso31661NumCode = "454", Domain = ".mw", InternationalPhoneCode = "265",
                Currency = "Kwacha malawiano", CurrencyCode = "MWK" },
            new() {
                Name = "Maldivas",
                Iso31661A2Code = "MV", Iso31661A3Code = "MDV", Iso31661NumCode = "462", Domain = ".mv", InternationalPhoneCode = "960",
                Currency = "Rupia maldiva", CurrencyCode = "MVR" },
            new() {
                Name = "Malí",
                Iso31661A2Code = "ML", Iso31661A3Code = "MLI", Iso31661NumCode = "466", Domain = ".ml", InternationalPhoneCode = "223",
                Currency = francoAfricaOccidental, CurrencyCode = "XOF" },
            new() {
                Name = "Malta",
                Iso31661A2Code = "MT", Iso31661A3Code = "MLT", Iso31661NumCode = "470", Domain = ".mt", InternationalPhoneCode = "356",
                Currency = "Euro", CurrencyCode = "EUR" },
            new() {
                Name = "Marruecos",
                Iso31661A2Code = "MA", Iso31661A3Code = "MAR", Iso31661NumCode = "504", Domain = ".ma", InternationalPhoneCode = "212",
                Currency = "Dirham marroquí", CurrencyCode = "MAD" },
            new() {
                Name = "Martinica",
                Iso31661A2Code = "MQ", Iso31661A3Code = "MTQ", Iso31661NumCode = "474", Domain = ".mq", InternationalPhoneCode = "596",
                Currency = "Euro", CurrencyCode = "EUR" },
            new() {
                Name = "Mauritania",
                Iso31661A2Code = "MR", Iso31661A3Code = "MRT", Iso31661NumCode = "478", Domain = ".mr", InternationalPhoneCode = "222",
                Currency = "Ouguiya mauritana", CurrencyCode = "MRU" },
            new() {
                Name = "Mayotte",
                Iso31661A2Code = "YT", Iso31661A3Code = "MYT", Iso31661NumCode = "175", Domain = ".yt", InternationalPhoneCode = "262",
                Currency = "Euro", CurrencyCode = "EUR" },
            new() {
                Name = "México",
                Iso31661A2Code = "MX", Iso31661A3Code = "MEX", Iso31661NumCode = "484", Domain = ".mx", InternationalPhoneCode = "52",
                Currency = "Peso mexicano", CurrencyCode = "MXN" },
            new() {
                Name = "Moldavia",
                Iso31661A2Code = "MD", Iso31661A3Code = "MDA", Iso31661NumCode = "498", Domain = ".md", InternationalPhoneCode = "376",
                Currency = "Leu moldavo", CurrencyCode = "MDL" },
            new() {
                Name = "Mongolia",
                Iso31661A2Code = "MN", Iso31661A3Code = "MNG", Iso31661NumCode = "496", Domain = ".mn", InternationalPhoneCode = "976",
                Currency = "Tugrik mongol", CurrencyCode = "MNT" },
            new() {
                Name = "Montenegro",
                Iso31661A2Code = "ME", Iso31661A3Code = "MNE", Iso31661NumCode = "499", Domain = ".me", InternationalPhoneCode = "382",
                Currency = "Euro", CurrencyCode = "EUR" },
            new() {
                Name = "Montserrat",
                Iso31661A2Code = "MS", Iso31661A3Code = "MSR", Iso31661NumCode = "500", Domain = ".ms", InternationalPhoneCode = "1-664",
                Currency = dolarCaribeOriental, CurrencyCode = "XCD" },
            new() {
                Name = "Mozambique",
                Iso31661A2Code = "MZ", Iso31661A3Code = "MOZ", Iso31661NumCode = "508", Domain = ".mz", InternationalPhoneCode = "259",
                Currency = "Metical mozambiqueño", CurrencyCode = "MZN" },
            new() {
                Name = "Myanmar",
                Iso31661A2Code = "MM", Iso31661A3Code = "MMR", Iso31661NumCode = "104", Domain = ".mm", InternationalPhoneCode = "95",
                Currency = "Kyat birmano", CurrencyCode = "MMK" },

            new() {
                Name = "Namibia",
                Iso31661A2Code = "NA", Iso31661A3Code = "NAM", Iso31661NumCode = "516", Domain = ".na", InternationalPhoneCode = "264",
                Currency = "Dólar namibio", CurrencyCode = "NAD" },
            new() {
                Name = "Nauru",
                Iso31661A2Code = "NR", Iso31661A3Code = "NRU", Iso31661NumCode = "520", Domain = ".nr", InternationalPhoneCode = "674",
                Currency = dolarAustraliano, CurrencyCode = "AUD" },
            new() {
                Name = "Nepal",
                Iso31661A2Code = "NP", Iso31661A3Code = "NPL", Iso31661NumCode = "524", Domain = ".np", InternationalPhoneCode = "977",
                Currency = "Rupia nepalí", CurrencyCode = "NPR" },
            new() {
                Name = "Nicaragua",
                Iso31661A2Code = "NI", Iso31661A3Code = "NIC", Iso31661NumCode = "558", Domain = ".ni", InternationalPhoneCode = "505",
                Currency = "Córdoba nicaragüense", CurrencyCode = "NIO" },
            new() {
                Name = "Níger",
                Iso31661A2Code = "NE", Iso31661A3Code = "NER", Iso31661NumCode = "562", Domain = ".ne", InternationalPhoneCode = "227",
                Currency = francoAfricaOccidental, CurrencyCode = "XOF" },
            new() {
                Name = "Nigeria",
                Iso31661A2Code = "NG", Iso31661A3Code = "NGA", Iso31661NumCode = "566", Domain = ".ng", InternationalPhoneCode = "234",
                Currency = "Naira nigeriana", CurrencyCode = "NGN" },
            new() {
                Name = "Niue",
                Iso31661A2Code = "NU", Iso31661A3Code = "NIU", Iso31661NumCode = "570", Domain = ".nu", InternationalPhoneCode = "683",
                Currency = dolarNeozelandes, CurrencyCode = "NZD" },
            new() {
                Name = "Noruega",
                Iso31661A2Code = "NO", Iso31661A3Code = "NOR", Iso31661NumCode = "578", Domain = ".no", InternationalPhoneCode = "47",
                Currency = "Corona noruega", CurrencyCode = "NOK" },
            new() {
                Name = "Nueva Caledonia",
                Iso31661A2Code = "NC", Iso31661A3Code = "NCL", Iso31661NumCode = "540", Domain = ".nc", InternationalPhoneCode = "687",
                Currency = "Franco CFP", CurrencyCode = "XPF" },
            new() {
                Name = "Nueva Zelanda",
                Iso31661A2Code = "NZ", Iso31661A3Code = "NZL", Iso31661NumCode = "554", Domain = ".nz", InternationalPhoneCode = "64",
                Currency = dolarNeozelandes, CurrencyCode = "NZD" },

            new() {
                Name = "Omán",
                Iso31661A2Code = "OM", Iso31661A3Code = "OMN", Iso31661NumCode = "512", Domain = ".om", InternationalPhoneCode = "968",
                Currency = "Rial omaní", CurrencyCode = "OMR" },

            new() {
                Name = "Países Bajos",
                Iso31661A2Code = "NL", Iso31661A3Code = "NLD", Iso31661NumCode = "528", Domain = ".nl", InternationalPhoneCode = "31",
                Currency = "Euro", CurrencyCode = "EUR" },
            new() {
                Name = "Pakistán",
                Iso31661A2Code = "PK", Iso31661A3Code = "PAK", Iso31661NumCode = "586", Domain = ".pk", InternationalPhoneCode = "92",
                Currency = "Rupia pakistaní", CurrencyCode = "PKR" },
            new() {
                Name = "Palaos",
                Iso31661A2Code = "PW", Iso31661A3Code = "PLW", Iso31661NumCode = "585", Domain = ".pw", InternationalPhoneCode = "680",
                Currency = dolarEstadounidense, CurrencyCode = "USD" },
            new() {
                Name = "Palestina",
                Iso31661A2Code = "PS", Iso31661A3Code = "PSE", Iso31661NumCode = "275", Domain = ".ps", InternationalPhoneCode = "970",
                Currency = "Dinares jordanos", CurrencyCode = "JOD" },
            new() {
                Name = "Panamá",
                Iso31661A2Code = "PA", Iso31661A3Code = "PAN", Iso31661NumCode = "591", Domain = ".pa", InternationalPhoneCode = "507",
                Currency = "Balboa panameño", CurrencyCode = "PAB" },
            new() {
                Name = "Papúa Nueva Guinea",
                Iso31661A2Code = "PG", Iso31661A3Code = "PNG", Iso31661NumCode = "598", Domain = ".pg", InternationalPhoneCode = "675",
                Currency = "Kina papú", CurrencyCode = "PGK" },
            new() {
                Name = "Paraguay",
                Iso31661A2Code = "PY", Iso31661A3Code = "PRY", Iso31661NumCode = "600", Domain = ".py", InternationalPhoneCode = "595",
                Currency = "Guaraní paraguayo", CurrencyCode = "PYG" },
            new() {
                Name = "Perú",
                Iso31661A2Code = "PE", Iso31661A3Code = "PER", Iso31661NumCode = "604", Domain = ".pe", InternationalPhoneCode = "51",
                Currency = "Sol peruano", CurrencyCode = "PEN" },
            new() {
                Name = "Polinesia Francesa",
                Iso31661A2Code = "PF", Iso31661A3Code = "PYF", Iso31661NumCode = "258", Domain = ".pf", InternationalPhoneCode = "689",
                Currency = "Franco CFP", CurrencyCode = "XPF" },
            new() {
                Name = "Polonia",
                Iso31661A2Code = "PL", Iso31661A3Code = "POL", Iso31661NumCode = "616", Domain = ".pl", InternationalPhoneCode = "48",
                Currency = "Zloty polaco", CurrencyCode = "PLN" },
            new() {
                Name = "Portugal",
                Iso31661A2Code = "PT", Iso31661A3Code = "PRT", Iso31661NumCode = "620", Domain = ".pt", InternationalPhoneCode = "351",
                Currency = "Euro", CurrencyCode = "EUR" },
            new() {
                Name = "Principado de Mónaco",
                Iso31661A2Code = "MC", Iso31661A3Code = "MCO", Iso31661NumCode = "492", Domain = ".mc", InternationalPhoneCode = "377",
                Currency = "Euro", CurrencyCode = "EUR" },
            new() {
                Name = "Puerto Rico",
                Iso31661A2Code = "PR", Iso31661A3Code = "PRI", Iso31661NumCode = "630", Domain = ".pr", InternationalPhoneCode = "1-787",
                Currency = dolarEstadounidense, CurrencyCode = "USD" },

            new() {
                Name = "Reino Unido",
                Iso31661A2Code = "GB", Iso31661A3Code = "GBR", Iso31661NumCode = "826", Domain = ".uk", InternationalPhoneCode = "44",
                Currency = "Libra esterlina", CurrencyCode = "GBP" },
            new() {
                Name = "República Centroafricana",
                Iso31661A2Code = "CF", Iso31661A3Code = "CAF", Iso31661NumCode = "140", Domain = ".cf", InternationalPhoneCode = "236",
                Currency = francoAfricaCentral, CurrencyCode = "XAF" },
            new() {
                Name = "República Checa",
                Iso31661A2Code = "CZ", Iso31661A3Code = "CZE", Iso31661NumCode = "203", Domain = ".cz", InternationalPhoneCode = "420",
                Currency = "Corona checa", CurrencyCode = "CZK" },
            new() {
                Name = "República del Congo",
                Iso31661A2Code = "CG", Iso31661A3Code = "COG", Iso31661NumCode = "178", Domain = ".cg", InternationalPhoneCode = "242",
                Currency = francoAfricaCentral, CurrencyCode = "XAF" },
            new() {
                Name = "República Democrática del Congo",
                Iso31661A2Code = "CD", Iso31661A3Code = "COD", Iso31661NumCode = "180", Domain = ".cd", InternationalPhoneCode = "243",
                Currency = "Franco congoleño", CurrencyCode = "CDF" },
            new() {
                Name = "República Dominicana",
                Iso31661A2Code = "DO", Iso31661A3Code = "DOM", Iso31661NumCode = "214", Domain = ".do", InternationalPhoneCode = "1-809",
                Currency = "Peso dominicano", CurrencyCode = "DOP" },
            new() {
                Name = "Reunión",
                Iso31661A2Code = "RE", Iso31661A3Code = "REU", Iso31661NumCode = "638", Domain = ".re", InternationalPhoneCode = "262",
                Currency = "Euro", CurrencyCode = "EUR" },
            new() {
                Name = "Ruanda",
                Iso31661A2Code = "RW", Iso31661A3Code = "RWA", Iso31661NumCode = "646", Domain = ".rw", InternationalPhoneCode = "250",
                Currency = "Franco ruandés", CurrencyCode = "RWF" },
            new() {
                Name = "Rumania",
                Iso31661A2Code = "RO", Iso31661A3Code = "ROU", Iso31661NumCode = "642", Domain = ".ro", InternationalPhoneCode = "40",
                Currency = "Leu rumano", CurrencyCode = "RON" },
            new() {
                Name = "Rusia",
                Iso31661A2Code = "RU", Iso31661A3Code = "RUS", Iso31661NumCode = "643", Domain = ".ru", InternationalPhoneCode = "7",
                Currency = "Rublo ruso", CurrencyCode = "RUB" },

            new() {
                Name = "Sáhara Occidental",
                Iso31661A2Code = "EH", Iso31661A3Code = "ESH", Iso31661NumCode = "732", Domain = ".eh", InternationalPhoneCode = "212",
                Currency = "Dirham marroquí", CurrencyCode = "MAD" },
            new() {
                Name = "Samoa",
                Iso31661A2Code = "WS", Iso31661A3Code = "WSM", Iso31661NumCode = "882", Domain = ".ws", InternationalPhoneCode = "685",
                Currency = "Tala samoano", CurrencyCode = "WST" },
            new() {
                Name = "Samoa Americana",
                Iso31661A2Code = "AS", Iso31661A3Code = "ASM", Iso31661NumCode = "016", Domain = ".as", InternationalPhoneCode = "1-684",
                Currency = dolarEstadounidense, CurrencyCode = "USD" },
            new() {
                Name = "San Cristóbal y Nieves",
                Iso31661A2Code = "KN", Iso31661A3Code = "KNA", Iso31661NumCode = "659", Domain = ".kn", InternationalPhoneCode = "1-869",
                Currency = dolarCaribeOriental, CurrencyCode = "XCD" },
            new() {
                Name = "San Marino",
                Iso31661A2Code = "SM", Iso31661A3Code = "SMR", Iso31661NumCode = "674", Domain = ".sm", InternationalPhoneCode = "378",
                Currency = "Euro", CurrencyCode = "EUR" },
            new() {
                Name = "San Pedro y Miquelón",
                Iso31661A2Code = "PM", Iso31661A3Code = "SPM", Iso31661NumCode = "666", Domain = ".pm", InternationalPhoneCode = "508",
                Currency = "Euro", CurrencyCode = "EUR" },
            new() {
                Name = "San Vicente y las Granadinas",
                Iso31661A2Code = "VC", Iso31661A3Code = "VCT", Iso31661NumCode = "670", Domain = ".vc", InternationalPhoneCode = "1-784",
                Currency = dolarCaribeOriental, CurrencyCode = "XCD" },
            new() {
                Name = "Santa Elena, Ascensión y Tristán de Acuña",
                Iso31661A2Code = "SH", Iso31661A3Code = "SHN", Iso31661NumCode = "654", Domain = ".sh", InternationalPhoneCode = "290",
                Currency = "Libra de Santa Elena", CurrencyCode = "SHP" },
            new() {
                Name = "Santa Lucía",
                Iso31661A2Code = "LC", Iso31661A3Code = "LCA", Iso31661NumCode = "662", Domain = ".lc", InternationalPhoneCode = "1-758",
                Currency = dolarCaribeOriental, CurrencyCode = "XCD" },
            new() {
                Name = "Santo Tomé y Príncipe",
                Iso31661A2Code = "ST", Iso31661A3Code = "STP", Iso31661NumCode = "678", Domain = ".st", InternationalPhoneCode = "239",
                Currency = "Dobra santotomense", CurrencyCode = "STN" },
            new() {
                Name = "Senegal",
                Iso31661A2Code = "SN", Iso31661A3Code = "SEN", Iso31661NumCode = "686", Domain = ".sn", InternationalPhoneCode = "221",
                Currency = francoAfricaOccidental, CurrencyCode = "XOF" },
            new() {
                Name = "Serbia",
                Iso31661A2Code = "RS", Iso31661A3Code = "SRB", Iso31661NumCode = "688", Domain = ".rs", InternationalPhoneCode = "381",
                Currency = "Dinar serbio", CurrencyCode = "RSD" },
            new() {
                Name = "Seychelles",
                Iso31661A2Code = "SC", Iso31661A3Code = "SYC", Iso31661NumCode = "690", Domain = ".sc", InternationalPhoneCode = "248",
                Currency = "Rupia seychellense", CurrencyCode = "SCR" },
            new() {
                Name = "Sierra Leona",
                Iso31661A2Code = "SL", Iso31661A3Code = "SLE", Iso31661NumCode = "694", Domain = ".sl", InternationalPhoneCode = "232",
                Currency = "Leone sierraleonés", CurrencyCode = "SLL" },
            new() {
                Name = "Singapur",
                Iso31661A2Code = "SG", Iso31661A3Code = "SGP", Iso31661NumCode = "702", Domain = ".sg", InternationalPhoneCode = "65",
                Currency = "Dólar de Singapur", CurrencyCode = "SGD" },
            new() {
                Name = "Sint Maarten",
                Iso31661A2Code = "SX", Iso31661A3Code = "SXM", Iso31661NumCode = "534", Domain = ".sx", InternationalPhoneCode = "1-721",
                Currency = "Florín antillano neerlandés", CurrencyCode = "ANG" },
            new() {
                Name = "Siria",
                Iso31661A2Code = "SY", Iso31661A3Code = "SYR", Iso31661NumCode = "760", Domain = ".sy", InternationalPhoneCode = "963",
                Currency = "Libra siria", CurrencyCode = "SYP" },
            new() {
                Name = "Somalia",
                Iso31661A2Code = "SO", Iso31661A3Code = "SOM", Iso31661NumCode = "706", Domain = ".so", InternationalPhoneCode = "252",
                Currency = "Chelín somalí", CurrencyCode = "SOS" },
            new() {
                Name = "Sri Lanka",
                Iso31661A2Code = "LK", Iso31661A3Code = "LKA", Iso31661NumCode = "144", Domain = ".lk", InternationalPhoneCode = "94",
                Currency = "Rupia de Sri Lanka", CurrencyCode = "LKR" },
            new() {
                Name = "Sudáfrica",
                Iso31661A2Code = "ZA", Iso31661A3Code = "ZAF", Iso31661NumCode = "710", Domain = ".za", InternationalPhoneCode = "27",
                Currency = "Rand sudafricano", CurrencyCode = "ZAR" },
            new() {
                Name = "Sudán",
                Iso31661A2Code = "SD", Iso31661A3Code = "SDN", Iso31661NumCode = "729", Domain = ".sd", InternationalPhoneCode = "249",
                Currency = "Libra sudanesa", CurrencyCode = "SDG" },
            new() {
                Name = "Sudán del Sur",
                Iso31661A2Code = "SS", Iso31661A3Code = "SSD", Iso31661NumCode = "728", Domain = ".ss", InternationalPhoneCode = "211",
                Currency = "Libra sursudanesa", CurrencyCode = "SSP" },
            new() {
                Name = "Suecia",
                Iso31661A2Code = "SE", Iso31661A3Code = "SWE", Iso31661NumCode = "752", Domain = ".se", InternationalPhoneCode = "46",
                Currency = "Corona sueca", CurrencyCode = "SEK" },
            new() {
                Name = "Suiza",
                Iso31661A2Code = "CH", Iso31661A3Code = "CHE", Iso31661NumCode = "756", Domain = ".ch", InternationalPhoneCode = "41",
                Currency = "Franco suizo", CurrencyCode = "CHF" },
            new() {
                Name = "Surinam",
                Iso31661A2Code = "SR", Iso31661A3Code = "SUR", Iso31661NumCode = "740", Domain = ".sr", InternationalPhoneCode = "597",
                Currency = "Dólar surinamés", CurrencyCode = "SRD" },
            new() {
                Name = "Svalbard y Jan Mayen",
                Iso31661A2Code = "SJ", Iso31661A3Code = "SJM", Iso31661NumCode = "744", Domain = ".sj", InternationalPhoneCode = "47",
                Currency = "Corona noruega", CurrencyCode = "NOK" },

            new() {
                Name = "Tailandia",
                Iso31661A2Code = "TH", Iso31661A3Code = "THA", Iso31661NumCode = "764", Domain = ".th", InternationalPhoneCode = "66",
                Currency = "Baht tailandés", CurrencyCode = "THB" },
            new() {
                Name = "Taiwán",
                Iso31661A2Code = "TW", Iso31661A3Code = "TWN", Iso31661NumCode = "158", Domain = ".tw", InternationalPhoneCode = "886",
                Currency = "Nuevo dólar taiwanés", CurrencyCode = "TWD" },
            new() {
                Name = "Tanzania",
                Iso31661A2Code = "TZ", Iso31661A3Code = "TZA", Iso31661NumCode = "834", Domain = ".tz", InternationalPhoneCode = "255",
                Currency = "Chelín tanzano", CurrencyCode = "TZS" },
            new() {
                Name = "Tayikistán",
                Iso31661A2Code = "TJ", Iso31661A3Code = "TJK", Iso31661NumCode = "762", Domain = ".tj", InternationalPhoneCode = "992",
                Currency = "Somoni tayiko", CurrencyCode = "TJS" },
            new() {
                Name = "Territorio Británico del Océano Índico",
                Iso31661A2Code = "IO", Iso31661A3Code = "IOT", Iso31661NumCode = "086", Domain = ".io", InternationalPhoneCode = "246",
                Currency = dolarEstadounidense, CurrencyCode = "USD" },
            new() {
                Name = "Territorios Australes Franceses",
                Iso31661A2Code = "TF", Iso31661A3Code = "ATF", Iso31661NumCode = "260", Domain = ".tf", InternationalPhoneCode = "33",
                Currency = "Euro", CurrencyCode = "EUR" },
            new() {
                Name = "Timor Oriental",
                Iso31661A2Code = "TL", Iso31661A3Code = "TLS", Iso31661NumCode = "626", Domain = ".tl", InternationalPhoneCode = "670",
                Currency = dolarEstadounidense, CurrencyCode = "USD" },
            new() {
                Name = "Togo",
                Iso31661A2Code = "TG", Iso31661A3Code = "TGO", Iso31661NumCode = "768", Domain = ".tg", InternationalPhoneCode = "228",
                Currency = francoAfricaOccidental, CurrencyCode = "XOF" },
            new() {
                Name = "Tokelau",
                Iso31661A2Code = "TK", Iso31661A3Code = "TKL", Iso31661NumCode = "772", Domain = ".tk", InternationalPhoneCode = "690",
                Currency = dolarNeozelandes, CurrencyCode = "NZD" },
            new() {
                Name = "Tonga",
                Iso31661A2Code = "TO", Iso31661A3Code = "TON", Iso31661NumCode = "776", Domain = ".to", InternationalPhoneCode = "676",
                Currency = "Paʻanga tongano", CurrencyCode = "TOP" },
            new() {
                Name = "Trinidad y Tobago",
                Iso31661A2Code = "TT", Iso31661A3Code = "TTO", Iso31661NumCode = "780", Domain = ".tt", InternationalPhoneCode = "1-868",
                Currency = "Dólar trinitense", CurrencyCode = "TTD" },
            new() {
                Name = "Túnez",
                Iso31661A2Code = "TN", Iso31661A3Code = "TUN", Iso31661NumCode = "788", Domain = ".tn", InternationalPhoneCode = "216",
                Currency = "Dinar tunecino", CurrencyCode = "TND" },
            new() {
                Name = "Turkmenistán",
                Iso31661A2Code = "TM", Iso31661A3Code = "TKM", Iso31661NumCode = "795", Domain = ".tm", InternationalPhoneCode = "993",
                Currency = "Manat turcomano", CurrencyCode = "TMT" },
            new() {
                Name = "Turquía",
                Iso31661A2Code = "TR", Iso31661A3Code = "TUR", Iso31661NumCode = "792", Domain = ".tr", InternationalPhoneCode = "90",
                Currency = "Lira turca", CurrencyCode = "TRY" },
            new() {
                Name = "Tuvalu",
                Iso31661A2Code = "TV", Iso31661A3Code = "TUV", Iso31661NumCode = "798", Domain = ".tv", InternationalPhoneCode = "688",
                Currency = dolarAustraliano, CurrencyCode = "AUD" },

            new() {
                Name = "Ucrania",
                Iso31661A2Code = "UA", Iso31661A3Code = "UKR", Iso31661NumCode = "804", Domain = ".ua", InternationalPhoneCode = "380",
                Currency = "Grivna ucraniana", CurrencyCode = "UAH" },
            new() {
                Name = "Uganda",
                Iso31661A2Code = "UG", Iso31661A3Code = "UGA", Iso31661NumCode = "800", Domain = ".ug", InternationalPhoneCode = "256",
                Currency = "Chelín ugandés", CurrencyCode = "UGX" },
            new() {
                Name = "Uruguay",
                Iso31661A2Code = "UY", Iso31661A3Code = "URY", Iso31661NumCode = "858", Domain = ".uy", InternationalPhoneCode = "598",
                Currency = "Peso uruguayo", CurrencyCode = "UYU" },
            new() {
                Name = "Uzbekistán",
                Iso31661A2Code = "UZ", Iso31661A3Code = "UZB", Iso31661NumCode = "860", Domain = ".uz", InternationalPhoneCode = "998",
                Currency = "Sum uzbeko", CurrencyCode = "UZS" },

            new() {
                Name = "Vanuatu",
                Iso31661A2Code = "VU", Iso31661A3Code = "VUT", Iso31661NumCode = "548", Domain = ".vu", InternationalPhoneCode = "678",
                Currency = "Vatu vanuatuense", CurrencyCode = "VUV" },
            new() {
                Name = "Venezuela",
                Iso31661A2Code = "VE", Iso31661A3Code = "VEN", Iso31661NumCode = "862", Domain = ".ve", InternationalPhoneCode = "58",
                Currency = "Bolívar venezolano", CurrencyCode = "VES" },
            new() {
                Name = "Vietnam",
                Iso31661A2Code = "VN", Iso31661A3Code = "VNM", Iso31661NumCode = "704", Domain = ".vn", InternationalPhoneCode = "84",
                Currency = "Dong vietnamita", CurrencyCode = "VND" },

            new() {
                Name = "Wallis y Futuna",
                Iso31661A2Code = "WF", Iso31661A3Code = "WLF", Iso31661NumCode = "876", Domain = ".wf", InternationalPhoneCode = "681",
                Currency = "Franco CFP", CurrencyCode = "XPF" },

            new() {
                Name = "Yemen",
                Iso31661A2Code = "YE", Iso31661A3Code = "YEM", Iso31661NumCode = "887", Domain = ".ye", InternationalPhoneCode = "967",
                Currency = "Rial yemení", CurrencyCode = "YER" },
            new() {
                Name = "Yibuti",
                Iso31661A2Code = "DJ", Iso31661A3Code = "DJI", Iso31661NumCode = "262", Domain = ".dj", InternationalPhoneCode = "253",
                Currency = "Franco yibutiano", CurrencyCode = "DJF" },

            new() {
                Name = "Zambia",
                Iso31661A2Code = "ZM", Iso31661A3Code = "ZMB", Iso31661NumCode = "894", Domain = ".zm", InternationalPhoneCode = "260",
                Currency = "Kwacha zambiano", CurrencyCode = "ZMW" },
            new() {
                Name = "Zimbabue",
                Iso31661A2Code = "ZW", Iso31661A3Code = "ZWE", Iso31661NumCode = "716", Domain = ".zw", InternationalPhoneCode = "263",
                Currency = "Dólar zimbabuense", CurrencyCode = "ZWL" },
        };

        // Basic validation before insertion
        var invalid = countries
            .Select((c, i) => new { Index = i, Country = c })
            .Where(x =>
                string.IsNullOrWhiteSpace(x.Country.Name) ||
                string.IsNullOrWhiteSpace(x.Country.Iso31661A2Code) ||
                string.IsNullOrWhiteSpace(x.Country.Iso31661A3Code) ||
                string.IsNullOrWhiteSpace(x.Country.Iso31661NumCode) ||
                string.IsNullOrWhiteSpace(x.Country.Currency) ||
                string.IsNullOrWhiteSpace(x.Country.CurrencyCode))
            .ToList();
        if (invalid.Count != 0)
        {
            throw new InvalidOperationException("Seed data contains invalid country entries. Please validate the seed source.");
        }

        await using var tx = await context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            await context.AddRangeAsync(countries, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            await tx.CommitAsync(cancellationToken);
        }
        catch
        {
            await tx.RollbackAsync(cancellationToken);
            throw;
        }
    }
}