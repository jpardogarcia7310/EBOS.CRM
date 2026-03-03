using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;

namespace EBOS.CRM.Infrastructure.Repositories.Concrete.CRM;

public class CustomerPrivacyRequestRepository(CrmDbContext context)
    : BaseRepository<CustomerPrivacyRequest>(context), ICustomerPrivacyRequestRepository
{
    public async Task<IReadOnlyCollection<CustomerPrivacyRequest>> GetByCustomerPagedAsync(long tenantId, long customerId,
        int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var safePageNumber = Math.Max(1, pageNumber);
        var safePageSize = Math.Max(1, pageSize);

        return await AsQueryable()
            .Where(x => x.TenantId == tenantId && x.CustomerId == customerId)
            .OrderByDescending(x => x.RequestedAt)
            .ThenByDescending(x => x.Id)
            .Skip((safePageNumber - 1) * safePageSize)
            .Take(safePageSize)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<CustomerPrivacyRequest>> GetByStatusPagedAsync(long tenantId, string status,
        int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var safePageNumber = Math.Max(1, pageNumber);
        var safePageSize = Math.Max(1, pageSize);
        var normalizedStatus = status.Trim().ToUpperInvariant();

        return await AsQueryable()
            .Where(x => x.TenantId == tenantId && x.Status == normalizedStatus)
            .OrderByDescending(x => x.RequestedAt)
            .ThenByDescending(x => x.Id)
            .Skip((safePageNumber - 1) * safePageSize)
            .Take(safePageSize)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public Task<int> CountByStatusAsync(long tenantId, string status, CancellationToken cancellationToken = default)
    {
        var normalizedStatus = status.Trim().ToUpperInvariant();
        return AsQueryable()
            .Where(x => x.TenantId == tenantId && x.Status == normalizedStatus)
            .CountAsync(cancellationToken);
    }

    public Task<CustomerPrivacyRequest?> GetActiveByCustomerAndTypeAsync(long tenantId, long customerId, string requestType,
        CancellationToken cancellationToken = default)
    {
        var normalizedType = requestType.Trim().ToUpperInvariant();
        return AsQueryable()
            .Where(x => x.TenantId == tenantId
                        && x.CustomerId == customerId
                        && x.RequestType == normalizedType
                        && (x.Status == CustomerPrivacyRequest.StatusPending ||
                            x.Status == CustomerPrivacyRequest.StatusInProgress))
            .OrderByDescending(x => x.RequestedAt)
            .ThenByDescending(x => x.Id)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
