using EBOS.CRM.Domain.Exceptions;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;
using EBOS.CRM.Domain.Interfaces.Services.CRM;

namespace EBOS.CRM.Application.Features.CRM.IndividualCustomer.Services;

public sealed class IndividualCustomerReferenceValidationService(
    IStatusRepository statusRepository,
    IIdentificationTypeRepository identificationTypeRepository,
    ICountryRepository countryRepository) : IIndividualCustomerReferenceValidationService
{
    public async Task EnsureReferencesAvailableAsync(
        long statusId,
        long identificationTypeId,
        long? countryId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _ = await statusRepository.GetByIdAsync(statusId, cancellationToken)
                ?? throw new DomainValidationException(
                    "Status not found for individual customer.",
                    "DOMAIN_VALIDATION_INDIVIDUAL_CUSTOMER_STATUS_NOT_FOUND");

            _ = await identificationTypeRepository.GetByIdAsync(identificationTypeId, cancellationToken)
                ?? throw new DomainValidationException(
                    "Identification type not found for individual customer.",
                    "DOMAIN_VALIDATION_INDIVIDUAL_CUSTOMER_IDENTIFICATION_TYPE_NOT_FOUND");

            if (!countryId.HasValue)
            {
                return;
            }

            _ = await countryRepository.GetByIdAsync(countryId.Value, cancellationToken)
                ?? throw new DomainValidationException(
                    "Country not found for individual customer.",
                    "DOMAIN_VALIDATION_INDIVIDUAL_CUSTOMER_COUNTRY_NOT_FOUND");
        }
        catch (Exception ex) when (
            ex is not DomainException &&
            DomainTransientFailureClassifier.TryClassify(ex, nameof(EnsureReferencesAvailableAsync), out _))
        {
            throw new TransientDomainFailureException(
                "Transient failure while resolving individual customer references.",
                "DOMAIN_TRANSIENT_INDIVIDUAL_CUSTOMER_REFERENCE_RESOLUTION",
                ex);
        }
    }
}
