namespace EBOS.CRM.Infrastructure.Options;

public sealed class CustomerDedupeOptions
{
    public const string SectionName = "CustomerDedupe";

    public int EmailWeight { get; init; } = 50;
    public int PhoneWeight { get; init; } = 30;
    public int TaxIdWeight { get; init; } = 40;
    public int IdentificationNumberWeight { get; init; } = 40;
}
