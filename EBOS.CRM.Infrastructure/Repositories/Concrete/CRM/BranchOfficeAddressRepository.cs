using System.Threading;
using System.Threading.Tasks;
using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;

namespace EBOS.CRM.Infrastructure.Repositories.Concrete.CRM;

public class BranchOfficeAddressRepository(CrmDbContext context) : BaseRepository<BranchOfficeAddress>(context),
    IBranchOfficeAddressRepository
{
    public async Task<BranchOfficeAddress?> GetCurrentPrimaryAsync(long branchOfficeId,
        CancellationToken cancellationToken = default)
    {
        return await AsQueryable()
            .Include(ba => ba.Address)
            .FirstOrDefaultAsync(
                ba => ba.BranchOfficeId == branchOfficeId && ba.IsPrimary && ba.IsCurrent,
                cancellationToken);
    }
}


