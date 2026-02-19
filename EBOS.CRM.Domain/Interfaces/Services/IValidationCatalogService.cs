namespace EBOS.CRM.Domain.Interfaces.Services;

public interface IValidationCatalogService
{
    Task<string?> GetPatternAsync(long tenantId, string key, CancellationToken cancellationToken = default);
}
