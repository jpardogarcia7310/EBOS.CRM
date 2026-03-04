using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;

namespace EBOS.CRM.Infrastructure.Repositories.Concrete.CRM;

public class CustomerMergeHistoryRepository(CrmDbContext context)
    : BaseRepository<CustomerMergeHistory>(context), ICustomerMergeHistoryRepository
{
    public async Task<IReadOnlyCollection<CustomerMergeHistory>> GetByWinnerPagedAsync(long tenantId, long winnerCustomerId,
        int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var safePageNumber = Math.Max(1, pageNumber);
        var safePageSize = Math.Max(1, pageSize);

        return await AsQueryable()
            .Where(x => x.TenantId == tenantId && x.WinnerCustomerId == winnerCustomerId)
            .OrderByDescending(x => x.CreatedAt)
            .ThenByDescending(x => x.Id)
            .Skip((safePageNumber - 1) * safePageSize)
            .Take(safePageSize)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public Task<int> CountByWinnerAsync(long tenantId, long winnerCustomerId, CancellationToken cancellationToken = default)
    {
        return AsQueryable()
            .Where(x => x.TenantId == tenantId && x.WinnerCustomerId == winnerCustomerId)
            .CountAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<CustomerMergeHistory>> GetByMergedPagedAsync(long tenantId, long mergedCustomerId,
        int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var safePageNumber = Math.Max(1, pageNumber);
        var safePageSize = Math.Max(1, pageSize);

        return await AsQueryable()
            .Where(x => x.TenantId == tenantId && x.MergedCustomerId == mergedCustomerId)
            .OrderByDescending(x => x.CreatedAt)
            .ThenByDescending(x => x.Id)
            .Skip((safePageNumber - 1) * safePageSize)
            .Take(safePageSize)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public Task<int> CountByMergedAsync(long tenantId, long mergedCustomerId, CancellationToken cancellationToken = default)
    {
        return AsQueryable()
            .Where(x => x.TenantId == tenantId && x.MergedCustomerId == mergedCustomerId)
            .CountAsync(cancellationToken);
    }
}
