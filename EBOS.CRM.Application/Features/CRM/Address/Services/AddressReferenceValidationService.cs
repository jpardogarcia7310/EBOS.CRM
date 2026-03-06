using EBOS.CRM.Domain.Exceptions;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;
using EBOS.CRM.Domain.Interfaces.Services.CRM;

namespace EBOS.CRM.Application.Features.CRM.Address.Services;

public sealed class AddressReferenceValidationService(
    ICountryRepository countryRepository,
    IAddressTypeRepository addressTypeRepository) : IAddressReferenceValidationService
{
    public async Task EnsureReferencesAvailableAsync(
        long countryId,
        long addressTypeId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _ = await countryRepository.GetByIdAsync(countryId, cancellationToken)
                ?? throw new DomainValidationException(
                    "Country not found for address.",
                    "DOMAIN_VALIDATION_ADDRESS_COUNTRY_NOT_FOUND");

            _ = await addressTypeRepository.GetByIdAsync(addressTypeId, cancellationToken)
                ?? throw new DomainValidationException(
                    "Address type not found.",
                    "DOMAIN_VALIDATION_ADDRESS_TYPE_NOT_FOUND");
        }
        catch (Exception ex) when (
            ex is not DomainException &&
            DomainTransientFailureClassifier.TryClassify(ex, nameof(EnsureReferencesAvailableAsync), out _))
        {
            throw new TransientDomainFailureException(
                "Transient failure while resolving address references.",
                "DOMAIN_TRANSIENT_ADDRESS_REFERENCE_RESOLUTION",
                ex);
        }
    }
}
