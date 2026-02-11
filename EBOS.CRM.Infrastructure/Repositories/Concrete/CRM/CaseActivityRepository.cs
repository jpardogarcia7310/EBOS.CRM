using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;

namespace EBOS.CRM.Infrastructure.Repositories.Concrete.CRM;

public class CaseActivityRepository(CrmDbContext context) : BaseRepository<CaseActivity>(context),
    ICaseActivityRepository
{
    public async Task<bool> HasOpenByCaseIdAsync(long caseId, CancellationToken cancellationToken = default)
    {
        return await AsQueryable()
            .Where(a => a.CaseId == caseId && a.Status != CaseActivity.StatusCompleted)
            .AnyAsync(cancellationToken);
    }
}
