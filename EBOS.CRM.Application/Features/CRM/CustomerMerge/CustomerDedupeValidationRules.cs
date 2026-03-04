namespace EBOS.CRM.Application.Features.CRM.CustomerMerge;

public static class CustomerDedupeValidationRules
{
    public const int MaxEmailLength = 100;
    public const int MaxPhoneDigits = 12;
    public const int MaxTaxIdLength = 20;
    public const int MaxIdentificationNumberLength = 10;

    public const string DigitsPattern = "^[0-9]+$";
    public const string AlphanumericPattern = "^[A-Za-z0-9]+$";
}
