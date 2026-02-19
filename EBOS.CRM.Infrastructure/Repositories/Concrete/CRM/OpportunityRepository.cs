using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;

namespace EBOS.CRM.Infrastructure.Repositories.Concrete.CRM;

public class OpportunityRepository(CrmDbContext context) : BaseRepository<Opportunity>(context), IOpportunityRepository
{
    private readonly CrmDbContext _context = context;

    public async Task<IReadOnlyCollection<Opportunity>> GetByForecastCriteriaAsync(
        long tenantId,
        long? ownerUserId,
        long? stageId,
        DateTime? from,
        DateTime? to,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Opportunities
            .AsNoTracking()
            .Where(opportunity => !opportunity.Erased && opportunity.TenantId == tenantId);

        if (ownerUserId.HasValue)
        {
            query = query.Where(opportunity => opportunity.OwnerUserId == ownerUserId.Value);
        }

        if (stageId.HasValue)
        {
            query = query.Where(opportunity => opportunity.StageId == stageId.Value);
        }

        if (from.HasValue)
        {
            query = query.Where(opportunity => opportunity.ExpectedCloseDate >= from.Value);
        }

        if (to.HasValue)
        {
            query = query.Where(opportunity => opportunity.ExpectedCloseDate <= to.Value);
        }

        return await query
            .OrderBy(opportunity => opportunity.ExpectedCloseDate)
            .ThenByDescending(opportunity => opportunity.Id)
            .ToListAsync(cancellationToken);
    }
}
