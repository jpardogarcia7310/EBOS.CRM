using System.Text.Json.Serialization;

namespace EBOS.CRM.Api.Controllers.Countries.Requests;

public record UpdateCountryRQ(
    [property: JsonPropertyName("name"), JsonRequired] string Name,
    [property: JsonPropertyName("iso31661A2Code"), JsonRequired] string Iso31661A2Code,
    [property: JsonPropertyName("iso31661A3Code"), JsonRequired] string Iso31661A3Code,
    [property: JsonPropertyName("iso31661NumCode"), JsonRequired] string Iso31661NumCode,
    [property: JsonPropertyName("domain"), JsonRequired] string Domain,
    [property: JsonPropertyName("currency"), JsonRequired] string Currency,
    [property: JsonPropertyName("currencyCode"), JsonRequired] string CurrencyCode,
    [property: JsonPropertyName("internationalPhoneCode"), JsonRequired] string InternationalPhoneCode
);