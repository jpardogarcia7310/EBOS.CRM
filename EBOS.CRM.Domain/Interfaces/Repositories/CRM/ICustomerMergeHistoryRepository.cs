using EBOS.Core.Primitives.Interfaces;
using EBOS.CRM.Domain.Entities.CRM;

namespace EBOS.CRM.Domain.Interfaces.Repositories.CRM;

public interface ICustomerMergeHistoryRepository : IPagedRepository<CustomerMergeHistory>, IUnitOfWork
{
    Task<CustomerMergeHistory?> GetLatestByMergedAsync(long tenantId, long mergedCustomerId,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<CustomerMergeHistory>> GetByWinnerPagedAsync(long tenantId, long winnerCustomerId,
        int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<int> CountByWinnerAsync(long tenantId, long winnerCustomerId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<CustomerMergeHistory>> GetByMergedPagedAsync(long tenantId, long mergedCustomerId,
        int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<int> CountByMergedAsync(long tenantId, long mergedCustomerId, CancellationToken cancellationToken = default);
}
