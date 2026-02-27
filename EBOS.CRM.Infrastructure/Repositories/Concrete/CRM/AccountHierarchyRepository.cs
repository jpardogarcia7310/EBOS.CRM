using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;

namespace EBOS.CRM.Infrastructure.Repositories.Concrete.CRM;

public class AccountHierarchyRepository(CrmDbContext context)
    : BaseRepository<AccountHierarchy>(context), IAccountHierarchyRepository
{
    public async Task<IReadOnlyCollection<AccountHierarchy>> GetByAccountPagedAsync(long tenantId,
        long corporateCustomerId, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var safePageNumber = Math.Max(1, pageNumber);
        var safePageSize = Math.Max(1, pageSize);

        return await AsQueryable()
            .Where(x => x.TenantId == tenantId &&
                        (x.ParentCorporateCustomerId == corporateCustomerId
                         || x.ChildCorporateCustomerId == corporateCustomerId))
            .OrderBy(x => x.Id)
            .Skip((safePageNumber - 1) * safePageSize)
            .Take(safePageSize)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public Task<int> CountByAccountAsync(long tenantId, long corporateCustomerId,
        CancellationToken cancellationToken = default)
        => AsQueryable()
            .Where(x => x.TenantId == tenantId &&
                        (x.ParentCorporateCustomerId == corporateCustomerId
                         || x.ChildCorporateCustomerId == corporateCustomerId))
            .CountAsync(cancellationToken);
}
