namespace EBOS.CRM.Domain.Interfaces.Services.CRM;

public interface IIndividualCustomerReferenceValidationService
{
    Task EnsureReferencesAvailableAsync(
        long statusId,
        long identificationTypeId,
        long? countryId,
        CancellationToken cancellationToken = default);
}
