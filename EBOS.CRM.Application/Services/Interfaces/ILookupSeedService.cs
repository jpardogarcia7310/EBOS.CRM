namespace EBOS.CRM.Application.Services.Interfaces;

public interface ILookupSeedService
{
    Task EnsureCanonicalLookupsAsync(CancellationToken cancellationToken = default);
}
