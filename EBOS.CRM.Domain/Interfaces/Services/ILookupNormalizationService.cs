namespace EBOS.CRM.Domain.Interfaces.Services;

public interface ILookupNormalizationService
{
    Task NormalizeAsync(CancellationToken cancellationToken = default);
}