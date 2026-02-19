using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;
using EBOS.CRM.Domain.Interfaces.Services;
using Microsoft.Extensions.Caching.Memory;

namespace EBOS.CRM.Infrastructure.Services.Validation;

public class ValidationCatalogService : IValidationCatalogService
{
    private readonly IValidationRuleRepository _repository;
    private readonly IMemoryCache _cache;

    public ValidationCatalogService(IValidationRuleRepository repository, IMemoryCache cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task<string?> GetPatternAsync(long tenantId, string key, CancellationToken cancellationToken = default)
    {
        if (tenantId <= 0 || string.IsNullOrWhiteSpace(key))
        {
            return null;
        }

        var normalizedKey = key.Trim();
        var cacheKey = $"validation_rule:{tenantId}:{normalizedKey}";

        if (_cache.TryGetValue(cacheKey, out string? cached))
        {
            return cached;
        }

        var rules = await _repository.GetByKeysAsync(tenantId, new[] { normalizedKey }, cancellationToken);
        var pattern = rules.FirstOrDefault()?.Pattern;

        _cache.Set(cacheKey, pattern, TimeSpan.FromMinutes(10));
        return pattern;
    }
}
