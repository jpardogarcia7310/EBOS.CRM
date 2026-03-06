namespace EBOS.CRM.Domain.Interfaces.Services.CRM;

public interface IAddressReferenceValidationService
{
    Task EnsureReferencesAvailableAsync(
        long countryId,
        long addressTypeId,
        CancellationToken cancellationToken = default);
}
