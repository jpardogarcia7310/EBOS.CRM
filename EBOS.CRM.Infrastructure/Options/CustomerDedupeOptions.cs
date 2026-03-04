namespace EBOS.CRM.Infrastructure.Options;

public sealed class CustomerDedupeOptions
{
    public const string SectionName = "CustomerDedupe";

    public int EmailWeight { get; init; } = 50;
    public int PhoneWeight { get; init; } = 30;
    public int PhoneApproxWeight { get; init; } = 15;
    public int TaxIdWeight { get; init; } = 40;
    public int IdentificationNumberWeight { get; init; } = 40;
    public bool EnablePhoneSuffixFallback { get; init; } = true;
    public int PhoneSuffixLength { get; init; } = 8;
    public int MinScore { get; init; } = 1;
}
