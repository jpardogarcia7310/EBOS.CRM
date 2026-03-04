namespace EBOS.CRM.Domain.Interfaces.Services.CRM;

public interface ICustomerDedupeNormalizationService
{
    string? NormalizeEmail(string? value);
    string? NormalizePhone(string? value);
    string? NormalizeAlphanumericUpper(string? value);
}
