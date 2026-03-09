using EBOS.CRM.Domain.Exceptions;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services.CRM;

namespace EBOS.CRM.Application.Features.CRM.AccountContactRole.Services;

public sealed class AccountContactRoleReferenceValidationService(IAccountContactRepository accountContactRepository) : IAccountContactRoleReferenceValidationService
{
    public async Task<global::EBOS.CRM.Domain.Entities.CRM.AccountContact> EnsureAccountContactAvailableAsync(long tenantId, long accountContactId, CancellationToken cancellationToken = default)
    {
        try
        {
            var accountContact = await accountContactRepository.GetByIdAsync(accountContactId, cancellationToken)
                ?? throw new DomainValidationException("Account contact not found.", "DOMAIN_VALIDATION_ACCOUNT_CONTACT_NOT_FOUND");
            if (accountContact.TenantId != tenantId)
            {
                throw new DomainConflictException("Account contact tenant mismatch.", "DOMAIN_CONFLICT_ACCOUNT_CONTACT_TENANT_MISMATCH");
            }

            return accountContact;
        }
        catch (Exception ex) when (
            ex is not DomainException &&
            DomainTransientFailureClassifier.TryClassify(ex, nameof(EnsureAccountContactAvailableAsync), out _))
        {
            throw new TransientDomainFailureException(
                "Transient failure while validating account contact reference.",
                "DOMAIN_TRANSIENT_ACCOUNT_CONTACT_ROLE_ACCOUNT_CONTACT_LOOKUP",
                ex);
        }
    }
}
