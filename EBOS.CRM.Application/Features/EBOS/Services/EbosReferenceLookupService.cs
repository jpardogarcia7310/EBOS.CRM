using EBOS.CRM.Domain.Entities.EBOS;
using EBOS.CRM.Domain.Exceptions;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;
using EBOS.CRM.Domain.Interfaces.Services.EBOS;

namespace EBOS.CRM.Application.Features.EBOS.Services;

public sealed class EbosReferenceLookupService(
    IAddressTypeRepository addressTypeRepository,
    IChannelCountryRepository channelCountryRepository,
    IChannelTypeRepository channelTypeRepository,
    ICountryRepository countryRepository,
    IIdentificationTypeRepository identificationTypeRepository,
    IStatusRepository statusRepository,
    ITenantConfigurationRepository tenantConfigurationRepository,
    ITenantQuotaRepository tenantQuotaRepository,
    ITenantUsageMetricRepository tenantUsageMetricRepository,
    IValidationRuleRepository validationRuleRepository) : IEbosReferenceLookupService
{
    public Task<global::EBOS.CRM.Domain.Entities.EBOS.AddressType?> GetAddressTypeByIdAsync(long id, CancellationToken cancellationToken = default)
        => RunLookupAsync(() => addressTypeRepository.GetByIdAsync(id, cancellationToken), "GetAddressTypeByIdAsync", "DOMAIN_TRANSIENT_EBOS_ADDRESS_TYPE_LOOKUP");

    public Task<global::EBOS.CRM.Domain.Entities.EBOS.ChannelCountry?> GetChannelCountryByIdAsync(long id, CancellationToken cancellationToken = default)
        => RunLookupAsync(() => channelCountryRepository.GetByIdAsync(id, cancellationToken), "GetChannelCountryByIdAsync", "DOMAIN_TRANSIENT_EBOS_CHANNEL_COUNTRY_LOOKUP");

    public Task<global::EBOS.CRM.Domain.Entities.EBOS.ChannelType?> GetChannelTypeByIdAsync(long id, CancellationToken cancellationToken = default)
        => RunLookupAsync(() => channelTypeRepository.GetByIdAsync(id, cancellationToken), "GetChannelTypeByIdAsync", "DOMAIN_TRANSIENT_EBOS_CHANNEL_TYPE_LOOKUP");

    public Task<Country?> GetCountryByIdAsync(long id, CancellationToken cancellationToken = default)
        => RunLookupAsync(() => countryRepository.GetByIdAsync(id, cancellationToken), "GetCountryByIdAsync", "DOMAIN_TRANSIENT_EBOS_COUNTRY_LOOKUP");

    public Task<global::EBOS.CRM.Domain.Entities.EBOS.IdentificationType?> GetIdentificationTypeByIdAsync(long id, CancellationToken cancellationToken = default)
        => RunLookupAsync(() => identificationTypeRepository.GetByIdAsync(id, cancellationToken), "GetIdentificationTypeByIdAsync", "DOMAIN_TRANSIENT_EBOS_IDENTIFICATION_TYPE_LOOKUP");

    public Task<Status?> GetStatusByIdAsync(long id, CancellationToken cancellationToken = default)
        => RunLookupAsync(() => statusRepository.GetByIdAsync(id, cancellationToken), "GetStatusByIdAsync", "DOMAIN_TRANSIENT_EBOS_STATUS_LOOKUP");

    public Task<global::EBOS.CRM.Domain.Entities.EBOS.TenantConfiguration?> GetTenantConfigurationByIdAsync(long id, CancellationToken cancellationToken = default)
        => RunLookupAsync(() => tenantConfigurationRepository.GetByIdAsync(id, cancellationToken), "GetTenantConfigurationByIdAsync", "DOMAIN_TRANSIENT_EBOS_TENANT_CONFIGURATION_LOOKUP");

    public Task<global::EBOS.CRM.Domain.Entities.EBOS.TenantQuota?> GetTenantQuotaByIdAsync(long id, CancellationToken cancellationToken = default)
        => RunLookupAsync(() => tenantQuotaRepository.GetByIdAsync(id, cancellationToken), "GetTenantQuotaByIdAsync", "DOMAIN_TRANSIENT_EBOS_TENANT_QUOTA_LOOKUP");

    public Task<global::EBOS.CRM.Domain.Entities.EBOS.TenantUsageMetric?> GetTenantUsageMetricByIdAsync(long id, CancellationToken cancellationToken = default)
        => RunLookupAsync(() => tenantUsageMetricRepository.GetByIdAsync(id, cancellationToken), "GetTenantUsageMetricByIdAsync", "DOMAIN_TRANSIENT_EBOS_TENANT_USAGE_METRIC_LOOKUP");

    public async Task<global::EBOS.CRM.Domain.Entities.EBOS.ValidationRule> GetValidationRuleByIdOrThrowAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await RunLookupAsync(
            () => validationRuleRepository.GetByIdAsync(id, cancellationToken),
            nameof(GetValidationRuleByIdOrThrowAsync),
            "DOMAIN_TRANSIENT_EBOS_VALIDATION_RULE_LOOKUP");

        return entity ?? throw new DomainValidationException("ValidationRule not found.", "DOMAIN_VALIDATION_EBOS_VALIDATION_RULE_NOT_FOUND");
    }

    public Task<IReadOnlyCollection<global::EBOS.CRM.Domain.Entities.EBOS.TenantConfiguration>> GetTenantConfigurationsPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        => RunValueLookupAsync(
            () => tenantConfigurationRepository.GetAllPagedAsync(pageNumber, pageSize, cancellationToken),
            nameof(GetTenantConfigurationsPagedAsync),
            "DOMAIN_TRANSIENT_EBOS_TENANT_CONFIGURATION_LIST");

    public Task<int> CountTenantConfigurationsAsync(CancellationToken cancellationToken = default)
        => RunValueLookupAsync(
            () => tenantConfigurationRepository.CountAsync(cancellationToken),
            nameof(CountTenantConfigurationsAsync),
            "DOMAIN_TRANSIENT_EBOS_TENANT_CONFIGURATION_COUNT");

    public Task<IReadOnlyCollection<global::EBOS.CRM.Domain.Entities.EBOS.TenantQuota>> GetTenantQuotasPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        => RunValueLookupAsync(
            () => tenantQuotaRepository.GetAllPagedAsync(pageNumber, pageSize, cancellationToken),
            nameof(GetTenantQuotasPagedAsync),
            "DOMAIN_TRANSIENT_EBOS_TENANT_QUOTA_LIST");

    public Task<int> CountTenantQuotasAsync(CancellationToken cancellationToken = default)
        => RunValueLookupAsync(
            () => tenantQuotaRepository.CountAsync(cancellationToken),
            nameof(CountTenantQuotasAsync),
            "DOMAIN_TRANSIENT_EBOS_TENANT_QUOTA_COUNT");

    public Task<IReadOnlyCollection<global::EBOS.CRM.Domain.Entities.EBOS.TenantUsageMetric>> GetTenantUsageMetricsPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        => RunValueLookupAsync(
            () => tenantUsageMetricRepository.GetAllPagedAsync(pageNumber, pageSize, cancellationToken),
            nameof(GetTenantUsageMetricsPagedAsync),
            "DOMAIN_TRANSIENT_EBOS_TENANT_USAGE_METRIC_LIST");

    public Task<int> CountTenantUsageMetricsAsync(CancellationToken cancellationToken = default)
        => RunValueLookupAsync(
            () => tenantUsageMetricRepository.CountAsync(cancellationToken),
            nameof(CountTenantUsageMetricsAsync),
            "DOMAIN_TRANSIENT_EBOS_TENANT_USAGE_METRIC_COUNT");

    public Task<IReadOnlyCollection<global::EBOS.CRM.Domain.Entities.EBOS.ValidationRule>> GetValidationRulesPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        => RunValueLookupAsync(
            () => validationRuleRepository.GetAllPagedAsync(pageNumber, pageSize, cancellationToken),
            nameof(GetValidationRulesPagedAsync),
            "DOMAIN_TRANSIENT_EBOS_VALIDATION_RULE_LIST");

    public Task<int> CountValidationRulesAsync(CancellationToken cancellationToken = default)
        => RunValueLookupAsync(
            () => validationRuleRepository.CountAsync(cancellationToken),
            nameof(CountValidationRulesAsync),
            "DOMAIN_TRANSIENT_EBOS_VALIDATION_RULE_COUNT");

    public Task<IReadOnlyCollection<global::EBOS.CRM.Domain.Entities.EBOS.TenantConfiguration>> GetTenantConfigurationsAsync(CancellationToken cancellationToken = default)
        => RunValueLookupAsync(
            () => tenantConfigurationRepository.GetAllAsync(cancellationToken),
            nameof(GetTenantConfigurationsAsync),
            "DOMAIN_TRANSIENT_EBOS_TENANT_CONFIGURATION_LIST");

    private static async Task<T?> RunLookupAsync<T>(
        Func<Task<T?>> operation,
        string operationName,
        string transientCode)
        where T : class
    {
        try
        {
            return await operation();
        }
        catch (Exception ex) when (
            ex is not DomainException &&
            DomainTransientFailureClassifier.TryClassify(ex, operationName, out _))
        {
            throw new TransientDomainFailureException(
                $"Transient failure while resolving EBOS reference in {operationName}.",
                transientCode,
                ex);
        }
    }

    private static async Task<T> RunValueLookupAsync<T>(
        Func<Task<T>> operation,
        string operationName,
        string transientCode)
    {
        try
        {
            return await operation();
        }
        catch (Exception ex) when (
            ex is not DomainException &&
            DomainTransientFailureClassifier.TryClassify(ex, operationName, out _))
        {
            throw new TransientDomainFailureException(
                $"Transient failure while resolving EBOS reference in {operationName}.",
                transientCode,
                ex);
        }
    }
}
