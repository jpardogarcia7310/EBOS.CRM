using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;

namespace EBOS.CRM.Infrastructure.Repositories.Concrete.CRM;

public class TaxInformationAddressRepository(CrmDbContext context) : BaseRepository<TaxInformationAddress>(context),
    ITaxInformationAddressRepository
{
    public async Task<TaxInformationAddress?> GetCurrentPrimaryAsync(long taxInformationId,
        CancellationToken cancellationToken = default)
    {
        return await AsQueryable()
            .Include(ta => ta.Address)
            .FirstOrDefaultAsync(
                ta => ta.TaxInformationId == taxInformationId && ta.IsPrimary && ta.IsCurrent,
                cancellationToken);
    }
}


