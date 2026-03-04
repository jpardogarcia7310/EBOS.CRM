using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;

namespace EBOS.CRM.Infrastructure.Repositories.Concrete.CRM;

public class CustomerAddressRepository(CrmDbContext context)
    : BaseRepository<CustomerAddress>(context), ICustomerAddressRepository
{
    public async Task<IReadOnlyCollection<CustomerAddress>> GetByCustomerIdsAsync(long tenantId,
        IReadOnlyCollection<long> customerIds, CancellationToken cancellationToken = default)
    {
        return await AsQueryable()
            .Where(x => x.TenantId == tenantId && customerIds.Contains(x.CustomerId))
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsPrimaryAddressForCustomerAsync(long customerId,
        CancellationToken cancellationToken = default)
    {
        return await AsQueryable()
            .AnyAsync(ca => ca.CustomerId == customerId && ca.IsPrimary && ca.IsCurrent, cancellationToken);
    }

    public async Task<CustomerAddress?> GetCurrentPrimaryAsync(long customerId,
        CancellationToken cancellationToken = default)
    {
        return await AsQueryable()
            .Where(ca => ca.CustomerId == customerId && ca.IsPrimary && ca.IsCurrent)
            .OrderByDescending(ca => ca.ValidFrom)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
