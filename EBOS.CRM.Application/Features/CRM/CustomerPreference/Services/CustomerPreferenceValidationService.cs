using EBOS.CRM.Domain.Exceptions;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;
using EBOS.CRM.Domain.Interfaces.Services.CRM;

namespace EBOS.CRM.Application.Features.CRM.CustomerPreference.Services;

public sealed class CustomerPreferenceValidationService(
    ICustomerRepository customerRepository,
    IChannelTypeRepository channelTypeRepository) : ICustomerPreferenceValidationService
{
    public async Task EnsureCustomerAndChannelAvailableAsync(long tenantId, long customerId, long channelId, CancellationToken cancellationToken = default)
    {
        try
        {
            var customer = await customerRepository.GetByIdAsync(customerId, cancellationToken)
                ?? throw new DomainValidationException("Customer not found.", "DOMAIN_VALIDATION_CUSTOMER_NOT_FOUND");
            if (customer.TenantId != tenantId)
            {
                throw new DomainConflictException("Customer tenant mismatch.", "DOMAIN_CONFLICT_CUSTOMER_TENANT_MISMATCH");
            }

            var channelType = await channelTypeRepository.GetByIdAsync(channelId, cancellationToken)
                ?? throw new DomainValidationException("Channel type not found.", "DOMAIN_VALIDATION_CHANNEL_TYPE_NOT_FOUND");
            if (!channelType.IsActive)
            {
                throw new DomainRuleViolationException("Channel type is not active.", "DOMAIN_RULE_VIOLATION_CHANNEL_TYPE_INACTIVE");
            }
        }
        catch (Exception ex) when (
            ex is not DomainException &&
            DomainTransientFailureClassifier.TryClassify(ex, nameof(EnsureCustomerAndChannelAvailableAsync), out _))
        {
            throw new TransientDomainFailureException(
                "Transient failure while resolving customer preference dependencies.",
                "DOMAIN_TRANSIENT_CUSTOMER_PREFERENCE_REFERENCE_RESOLUTION",
                ex);
        }
    }
}
