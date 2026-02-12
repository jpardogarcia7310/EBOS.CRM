namespace EBOS.CRM.Domain.Interfaces.Services;

public interface ILookupSeedService
{
    Task EnsureCanonicalLookupsAsync(CancellationToken cancellationToken = default);
}