using EBOS.Core.Primitives.Interfaces;
using EBOS.CRM.Domain.Entities.CRM;

namespace EBOS.CRM.Domain.Interfaces.Repositories.CRM;

public interface ICaseRepository : IPagedRepository<Case>, IUnitOfWork
{
    Task<IReadOnlyCollection<Case>> GetOpenSlaBatchAsync(long tenantId, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<int> CountOpenSlaBatchAsync(long tenantId, CancellationToken cancellationToken = default);
    Task<int> CountOpenByQueueIdAsync(long queueId, CancellationToken cancellationToken = default);
    Task<int> CountOpenBySlaIdAsync(long slaId, CancellationToken cancellationToken = default);
}
