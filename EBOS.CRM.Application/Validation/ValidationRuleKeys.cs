namespace EBOS.CRM.Application.Validation;

public static class ValidationRuleKeys
{
    public const string DefaultCountryKey = "DEFAULT";

    public static string PostalCode(string iso2) => $"postal_code:{iso2}";
    public static string Phone(string iso2) => $"phone:{iso2}";
    public static string TaxId(string iso2) => $"tax_id:{iso2}";
    public static string Identification(string typeCode) => $"id:{typeCode}";
}
