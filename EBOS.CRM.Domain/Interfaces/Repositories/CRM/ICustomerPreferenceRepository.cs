using EBOS.Core.Primitives.Interfaces;
using EBOS.CRM.Domain.Entities.CRM;

namespace EBOS.CRM.Domain.Interfaces.Repositories.CRM;

public interface ICustomerPreferenceRepository : IPagedRepository<CustomerPreference>, IUnitOfWork
{
    Task<IReadOnlyCollection<CustomerPreference>> GetByCustomerIdsAsync(long tenantId, IReadOnlyCollection<long> customerIds,
        CancellationToken cancellationToken = default);
    Task<CustomerPreference?> GetByCustomerAndChannelAsync(long tenantId, long customerId, long channelId,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<CustomerPreference>> GetByCustomerPagedAsync(long tenantId, long customerId,
        int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<int> CountByCustomerAsync(long tenantId, long customerId, CancellationToken cancellationToken = default);
}
