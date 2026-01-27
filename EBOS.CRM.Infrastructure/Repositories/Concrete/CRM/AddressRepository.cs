using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EBOS.CRM.Infrastructure.Repositories.Concrete.CRM;

public class AddressRepository(CrmDbContext context) : BaseRepository<Address>(context), IAddressRepository
{
    public async Task<bool> ExistPrimaryAddressInCustomerId(long customerId, 
        CancellationToken cancellationToken = default)
    {
        return await DbSet.AsNoTracking().AnyAsync(address => address.CustomerId == customerId && address.IsPrimary,
            cancellationToken);
    }

}
