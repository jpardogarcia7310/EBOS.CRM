using EBOS.Core.Primitives.Interfaces;
using EBOS.CRM.Domain.Entities.EBOS;

namespace EBOS.CRM.Domain.Interfaces.Repositories.EBOS;

public interface IChannelCountryRepository : IReadOnlyPagedRepository<ChannelCountry>
{
    Task<bool> IsAllowedAsync(long channelTypeId, long countryId, CancellationToken cancellationToken);
}
