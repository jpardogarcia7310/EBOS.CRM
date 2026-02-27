using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;

namespace EBOS.CRM.Infrastructure.Repositories.Concrete.CRM;

public class CustomerPreferenceRepository(CrmDbContext context)
    : BaseRepository<CustomerPreference>(context), ICustomerPreferenceRepository
{
    public async Task<IReadOnlyCollection<CustomerPreference>> GetByCustomerIdsAsync(long tenantId,
        IReadOnlyCollection<long> customerIds, CancellationToken cancellationToken = default)
    {
        return await AsQueryable()
            .Where(x => x.TenantId == tenantId && customerIds.Contains(x.CustomerId))
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public Task<CustomerPreference?> GetByCustomerAndChannelAsync(long tenantId, long customerId, long channelId,
        CancellationToken cancellationToken = default)
        => AsQueryable()
            .FirstOrDefaultAsync(x => x.TenantId == tenantId && x.CustomerId == customerId && x.ChannelId == channelId,
                cancellationToken);

    public async Task<IReadOnlyCollection<CustomerPreference>> GetByCustomerPagedAsync(long tenantId, long customerId,
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
