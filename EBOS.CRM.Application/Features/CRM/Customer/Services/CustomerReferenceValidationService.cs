using EBOS.CRM.Domain.Exceptions;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;
using EBOS.CRM.Domain.Interfaces.Services.CRM;

namespace EBOS.CRM.Application.Features.CRM.Customer.Services;

public sealed class CustomerReferenceValidationService(
    IStatusRepository statusRepository,
    ICountryRepository countryRepository) : ICustomerReferenceValidationService
{
    public async Task EnsureStatusAndCountryAvailableAsync(
        long statusId,
        long? countryId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var status = await statusRepository.GetByIdAsync(statusId, cancellationToken)
                ?? throw new DomainValidationException(
                    "Customer status not found.",
                    "DOMAIN_VALIDATION_CUSTOMER_STATUS_NOT_FOUND");

            if (status.Id <= 0)
            {
                throw new DomainRuleViolationException(
                    "Customer status is invalid.",
                    "DOMAIN_RULE_CUSTOMER_STATUS_INVALID");
            }

            if (!countryId.HasValue)
            {
                return;
            }

            var country = await countryRepository.GetByIdAsync(countryId.Value, cancellationToken)
                ?? throw new DomainValidationException(
                    "Country not found.",
                    "DOMAIN_VALIDATION_CUSTOMER_COUNTRY_NOT_FOUND");

            if (country.Id <= 0)
            {
                throw new DomainRuleViolationException(
                    "Country is invalid.",
                    "DOMAIN_RULE_CUSTOMER_COUNTRY_INVALID");
            }
        }
        catch (Exception ex) when (
            ex is not DomainException &&
            DomainTransientFailureClassifier.TryClassify(ex, nameof(EnsureStatusAndCountryAvailableAsync), out _))
        {
            throw new TransientDomainFailureException(
                "Transient failure while resolving customer references.",
                "DOMAIN_TRANSIENT_CUSTOMER_REFERENCE_RESOLUTION",
                ex);
        }
    }
}
