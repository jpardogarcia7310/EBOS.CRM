namespace EBOS.CRM.Domain.Interfaces.Services.CRM;

public interface ICustomerReferenceValidationService
{
    Task EnsureStatusAndCountryAvailableAsync(
        long statusId,
        long? countryId,
        CancellationToken cancellationToken = default);
}
