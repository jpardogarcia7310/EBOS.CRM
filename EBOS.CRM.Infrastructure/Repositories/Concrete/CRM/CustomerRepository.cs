using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Infrastructure.Persistence;


namespace EBOS.CRM.Infrastructure.Repositories.Concrete.CRM;

public class CustomerRepository(CrmDbContext context) : BaseRepository<Customer>(context), ICustomerRepository
{
    public async Task<Customer?> GetWithAddressesAsync(long id, CancellationToken cancellationToken = default)
    {
        return await AsQueryable()
            .Include(c => c.CustomerAddresses)
            .ThenInclude(ca => ca.Address)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }
}


