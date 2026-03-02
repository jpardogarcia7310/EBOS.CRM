using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;

namespace EBOS.CRM.Infrastructure.Repositories.Concrete.CRM;

public class CustomerConsentRepository(CrmDbContext context)
    : BaseRepository<CustomerConsent>(context), ICustomerConsentRepository
{
    public async Task<IReadOnlyCollection<CustomerConsent>> GetByCustomerIdsAsync(long tenantId,
        IReadOnlyCollection<long> customerIds, CancellationToken cancellationToken = default)
    {
        return await AsQueryable()
            .Where(x => x.TenantId == tenantId && customerIds.Contains(x.CustomerId))
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<CustomerConsent>> GetLatestByCustomerPagedAsync(long tenantId, long customerId,
        int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var safePageNumber = Math.Max(1, pageNumber);
        var safePageSize = Math.Max(1, pageSize);

        var baseQuery = AsQueryable()
            .Where(x => x.TenantId == tenantId && x.CustomerId == customerId);

        var latestDatesByType = baseQuery
            .GroupBy(x => x.ConsentType)
            .Select(g => new
            {
                ConsentType = g.Key,
                LatestAt = g.Max(x => x.RevokedAt ?? x.GrantedAt)
            });

        var latestIdsByType = baseQuery
            .Join(latestDatesByType,
                consent => new { consent.ConsentType, EventAt = consent.RevokedAt ?? consent.GrantedAt },
                latest => new { latest.ConsentType, EventAt = latest.LatestAt },
                (consent, _) => consent)
            .GroupBy(x => x.ConsentType)
            .Select(g => g.Max(x => x.Id));

        return await baseQuery
            .Where(x => latestIdsByType.Contains(x.Id))
            .OrderByDescending(x => x.RevokedAt ?? x.GrantedAt)
            .ThenByDescending(x => x.Id)
            .Skip((safePageNumber - 1) * safePageSize)
            .Take(safePageSize)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

    }

    public async Task<IReadOnlyCollection<CustomerConsent>> GetByCustomerPagedAsync(long tenantId, long customerId,
        int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var safePageNumber = Math.Max(1, pageNumber);
        var safePageSize = Math.Max(1, pageSize);

        return await AsQueryable()
            .Where(x => x.TenantId == tenantId && x.CustomerId == customerId)
            .OrderBy(x => x.Id)
            .Skip((safePageNumber - 1) * safePageSize)
            .Take(safePageSize)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountLatestByCustomerAsync(long tenantId, long customerId,
        CancellationToken cancellationToken = default)
    {
        var latestTypes = await AsQueryable()
            .Where(x => x.TenantId == tenantId && x.CustomerId == customerId)
            .Select(x => x.ConsentType)
            .Distinct()
            .CountAsync(cancellationToken);

        return latestTypes;
    }

    public Task<int> CountByCustomerAsync(long tenantId, long customerId, CancellationToken cancellationToken = default)
        => AsQueryable()
            .Where(x => x.TenantId == tenantId && x.CustomerId == customerId)
            .CountAsync(cancellationToken);
}
