namespace EBOS.CRM.Application.Services.Interfaces;

public interface ILookupNormalizationService
{
    Task NormalizeAsync(CancellationToken cancellationToken = default);
}
