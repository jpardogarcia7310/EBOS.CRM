using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;

namespace EBOS.CRM.Infrastructure.Repositories.Concrete.CRM;

public class AccountContactRoleRepository(CrmDbContext context)
    : BaseRepository<AccountContactRole>(context), IAccountContactRoleRepository
{
    public async Task<IReadOnlyCollection<AccountContactRole>> GetByAccountContactPagedAsync(long tenantId,
        long accountContactId, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var safePageNumber = Math.Max(1, pageNumber);
        var safePageSize = Math.Max(1, pageSize);

        return await AsQueryable()
            .Where(x => x.TenantId == tenantId && x.AccountContactId == accountContactId)
            .OrderBy(x => x.Id)
            .Skip((safePageNumber - 1) * safePageSize)
            .Take(safePageSize)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public Task<int> CountByAccountContactAsync(long tenantId, long accountContactId,
        CancellationToken cancellationToken = default)
        => AsQueryable()
            .Where(x => x.TenantId == tenantId && x.AccountContactId == accountContactId)
            .CountAsync(cancellationToken);
}
