namespace EBOS.CRM.Application.Options;

public sealed class ValidationCatalogOptions
{
    public const string SectionName = "ValidationCatalogs";

    public string? DefaultCountryIso2 { get; init; }
    public Dictionary<string, string> PostalCodePatternsByCountry { get; init; } = new();
    public Dictionary<string, string> PhonePatternsByCountry { get; init; } = new();
    public Dictionary<string, string> TaxIdPatternsByCountry { get; init; } = new();
    public Dictionary<string, string> IdentificationPatternsByTypeCode { get; init; } = new();
}
