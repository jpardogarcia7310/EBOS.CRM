using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;

namespace EBOS.CRM.Infrastructure.Repositories.Concrete.CRM;

public class CaseRepository(CrmDbContext context) : BaseRepository<Case>(context), ICaseRepository
{
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
