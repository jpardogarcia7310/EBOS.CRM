using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;

namespace EBOS.CRM.Infrastructure.Repositories.Concrete.CRM;

public class CaseRepository(CrmDbContext context) : BaseRepository<Case>(context), ICaseRepository
{
    public async Task<IReadOnlyCollection<Case>> GetOpenSlaBatchAsync(long tenantId, int pageNumber, int pageSize,
        CancellationToken cancellationToken = default)
    {
        var safePageNumber = Math.Max(1, pageNumber);
        var safePageSize = Math.Max(1, pageSize);

        return await AsQueryable()
            .Where(c => c.TenantId == tenantId && c.Status != Case.StatusClosed)
            .OrderBy(c => c.Id)
            .Skip((safePageNumber - 1) * safePageSize)
            .Take(safePageSize)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public Task<int> CountOpenSlaBatchAsync(long tenantId, CancellationToken cancellationToken = default)
        => AsQueryable()
            .Where(c => c.TenantId == tenantId && c.Status != Case.StatusClosed)
            .CountAsync(cancellationToken);

    public async Task<int> CountOpenByQueueIdAsync(long queueId, CancellationToken cancellationToken = default)
    {
        return await AsQueryable()
            .Where(c => c.QueueId == queueId && c.Status != Case.StatusClosed)
            .CountAsync(cancellationToken);
    }

    public async Task<int> CountOpenBySlaIdAsync(long slaId, CancellationToken cancellationToken = default)
    {
        return await AsQueryable()
            .Where(c => c.SlaId == slaId && c.Status != Case.StatusClosed)
            .CountAsync(cancellationToken);
    }
}
