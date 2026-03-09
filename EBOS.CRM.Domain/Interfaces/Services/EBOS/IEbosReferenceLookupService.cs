using EBOS.CRM.Domain.Entities.EBOS;

namespace EBOS.CRM.Domain.Interfaces.Services.EBOS;

public interface IEbosReferenceLookupService
{
    Task<AddressType?> GetAddressTypeByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<AddressType>> GetAddressTypesPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<int> CountAddressTypesAsync(CancellationToken cancellationToken = default);
    Task<ChannelCountry?> GetChannelCountryByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<ChannelCountry>> GetChannelCountriesPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<int> CountChannelCountriesAsync(CancellationToken cancellationToken = default);
    Task<ChannelType?> GetChannelTypeByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<ChannelType>> GetChannelTypesPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<int> CountChannelTypesAsync(CancellationToken cancellationToken = default);
    Task<Country?> GetCountryByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Country>> GetCountriesPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<int> CountCountriesAsync(CancellationToken cancellationToken = default);
    Task<IdentificationType?> GetIdentificationTypeByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<IdentificationType>> GetIdentificationTypesPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<int> CountIdentificationTypesAsync(CancellationToken cancellationToken = default);
    Task<Status?> GetStatusByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Status>> GetStatusesPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<int> CountStatusesAsync(CancellationToken cancellationToken = default);
    Task<TenantConfiguration?> GetTenantConfigurationByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<TenantQuota?> GetTenantQuotaByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<TenantUsageMetric?> GetTenantUsageMetricByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<ValidationRule> GetValidationRuleByIdOrThrowAsync(long id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<TenantConfiguration>> GetTenantConfigurationsPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<int> CountTenantConfigurationsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<TenantQuota>> GetTenantQuotasPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<int> CountTenantQuotasAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<TenantUsageMetric>> GetTenantUsageMetricsPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<int> CountTenantUsageMetricsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<ValidationRule>> GetValidationRulesPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<int> CountValidationRulesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<TenantConfiguration>> GetTenantConfigurationsAsync(CancellationToken cancellationToken = default);
}
