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

    public Task<int> CountByCustomerAsync(long tenantId, long customerId, CancellationToken cancellationToken = default)
        => AsQueryable()
            .Where(x => x.TenantId == tenantId && x.CustomerId == customerId)
            .CountAsync(cancellationToken);
}
