using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;

namespace EBOS.CRM.Infrastructure.Repositories.Concrete.CRM;

public class CustomerAddressRepository(CrmDbContext context) : BaseRepository<CustomerAddress>(context),
    ICustomerAddressRepository
{
    public async Task<bool> ExistsPrimaryAddressForCustomerAsync(long customerId, 
        CancellationToken cancellationToken = default)
    {
        return await AsQueryable()
            .Where(ca => ca.CustomerId == customerId && ca.IsPrimary && ca.IsCurrent)
            .AnyAsync(cancellationToken);
    }

    public async Task<CustomerAddress?> GetCurrentPrimaryAsync(long customerId,
        CancellationToken cancellationToken = default)
    {
        return await AsQueryable()
            .Include(ca => ca.Address)
            .FirstOrDefaultAsync(ca => ca.CustomerId == customerId && ca.IsPrimary && ca.IsCurrent, cancellationToken);
    }
}


