using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;

namespace EBOS.CRM.Infrastructure.Repositories.Concrete.CRM;

public class AccountContactRepository(CrmDbContext context)
    : BaseRepository<AccountContact>(context), IAccountContactRepository
{
    public async Task<IReadOnlyCollection<AccountContact>> GetByCorporateCustomerPagedAsync(long tenantId,
        long corporateCustomerId, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var safePageNumber = Math.Max(1, pageNumber);
        var safePageSize = Math.Max(1, pageSize);

        return await AsQueryable()
            .Where(x => x.TenantId == tenantId && x.CorporateCustomerId == corporateCustomerId)
            .OrderBy(x => x.Id)
            .Skip((safePageNumber - 1) * safePageSize)
            .Take(safePageSize)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public Task<int> CountByCorporateCustomerAsync(long tenantId, long corporateCustomerId,
        CancellationToken cancellationToken = default)
        => AsQueryable()
            .Where(x => x.TenantId == tenantId && x.CorporateCustomerId == corporateCustomerId)
            .CountAsync(cancellationToken);
}
