namespace EBOS.CRM.Domain.Interfaces.Services;

public interface IValidationCatalogService
{
    Task<string?> GetPatternAsync(string key, CancellationToken cancellationToken = default);
}
