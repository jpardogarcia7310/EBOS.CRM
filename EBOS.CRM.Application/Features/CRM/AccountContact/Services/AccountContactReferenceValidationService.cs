using EBOS.CRM.Domain.Exceptions;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services.CRM;

namespace EBOS.CRM.Application.Features.CRM.AccountContact.Services;

public sealed class AccountContactReferenceValidationService(
    ICorporateCustomerRepository corporateCustomerRepository,
    IIndividualCustomerRepository individualCustomerRepository) : IAccountContactReferenceValidationService
{
    public async Task<global::EBOS.CRM.Domain.Entities.CRM.CorporateCustomer> EnsureCorporateCustomerAvailableAsync(long tenantId, long corporateCustomerId, CancellationToken cancellationToken = default)
    {
        try
        {
            var corporateCustomer = await corporateCustomerRepository.GetByIdAsync(corporateCustomerId, cancellationToken)
                ?? throw new DomainValidationException("Corporate customer not found.", "DOMAIN_VALIDATION_CORPORATE_CUSTOMER_NOT_FOUND");
            if (corporateCustomer.TenantId != tenantId)
            {
                throw new DomainConflictException("Corporate customer tenant mismatch.", "DOMAIN_CONFLICT_CORPORATE_CUSTOMER_TENANT_MISMATCH");
            }

            return corporateCustomer;
        }
        catch (Exception ex) when (
            ex is not DomainException &&
            DomainTransientFailureClassifier.TryClassify(ex, nameof(EnsureCorporateCustomerAvailableAsync), out _))
        {
            throw new TransientDomainFailureException(
                "Transient failure while validating corporate customer reference.",
                "DOMAIN_TRANSIENT_ACCOUNT_CONTACT_CORPORATE_CUSTOMER_LOOKUP",
                ex);
        }
    }

    public async Task<global::EBOS.CRM.Domain.Entities.CRM.IndividualCustomer> EnsureIndividualCustomerAvailableAsync(long tenantId, long individualCustomerId, CancellationToken cancellationToken = default)
    {
        try
        {
            var individualCustomer = await individualCustomerRepository.GetByIdAsync(individualCustomerId, cancellationToken)
                ?? throw new DomainValidationException("Individual customer not found.", "DOMAIN_VALIDATION_INDIVIDUAL_CUSTOMER_NOT_FOUND");
            if (individualCustomer.TenantId != tenantId)
            {
                throw new DomainConflictException("Individual customer tenant mismatch.", "DOMAIN_CONFLICT_INDIVIDUAL_CUSTOMER_TENANT_MISMATCH");
            }

            return individualCustomer;
        }
        catch (Exception ex) when (
            ex is not DomainException &&
            DomainTransientFailureClassifier.TryClassify(ex, nameof(EnsureIndividualCustomerAvailableAsync), out _))
        {
            throw new TransientDomainFailureException(
                "Transient failure while validating individual customer reference.",
                "DOMAIN_TRANSIENT_ACCOUNT_CONTACT_INDIVIDUAL_CUSTOMER_LOOKUP",
                ex);
        }
    }
}
