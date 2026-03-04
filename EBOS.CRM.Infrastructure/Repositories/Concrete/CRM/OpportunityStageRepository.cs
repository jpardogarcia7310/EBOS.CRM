using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;

namespace EBOS.CRM.Infrastructure.Repositories.Concrete.CRM;

public class OpportunityStageRepository(CrmDbContext context)
    : BaseRepository<OpportunityStage>(context), IOpportunityStageRepository
{
    private readonly CrmDbContext _context = context;

    public async Task<IReadOnlyCollection<OpportunityStage>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.OpportunityStages
            .AsNoTracking()
            .Where(stage => !stage.Erased)
            .OrderBy(stage => stage.Order)
            .ToListAsync(cancellationToken);
    }
}
