using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;

namespace EBOS.CRM.Infrastructure.Repositories.Concrete.CRM;

public class SlaRepository(CrmDbContext context) : BaseRepository<Sla>(context), ISlaRepository
{
    private readonly CrmDbContext _context = context;

    public async Task<IReadOnlyCollection<Sla>> GetByIdsAsync(IReadOnlyCollection<long> ids, CancellationToken cancellationToken = default)
    {
        if (ids.Count == 0)
        {
            return Array.Empty<Sla>();
        }

        return await _context.Slas
            .AsNoTracking()
            .Where(sla => !sla.Erased && ids.Contains(sla.Id))
            .ToListAsync(cancellationToken);
    }
}
