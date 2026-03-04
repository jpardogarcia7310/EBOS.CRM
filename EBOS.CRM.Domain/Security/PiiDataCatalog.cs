namespace EBOS.CRM.Domain.Security;

public static class PiiDataCatalog
{
    public const string CustomerEmail = "crm.customer.email";
    public const string CustomerPhone = "crm.customer.phone";
    public const string CorporateTaxIdentification = "crm.corporate.taxIdentification";
    public const string IndividualIdentificationNumber = "crm.individual.identificationNumber";
    public const string IndividualFirstName = "crm.individual.firstName";
    public const string IndividualLastName = "crm.individual.lastName";

    public static readonly string[] Customer360DefaultFields =
    {
        CustomerEmail,
        CustomerPhone,
        CorporateTaxIdentification,
        IndividualIdentificationNumber,
        IndividualFirstName,
        IndividualLastName
    };
}
