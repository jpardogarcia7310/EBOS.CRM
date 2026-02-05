using System.Threading;
using System.Threading.Tasks;
using EBOS.Core.Primitives.Interfaces;
using EBOS.CRM.Domain.Entities.CRM;

namespace EBOS.CRM.Domain.Interfaces.Repositories.CRM;

public interface IBranchOfficeAddressRepository : IPagedRepository<BranchOfficeAddress>, IUnitOfWork
{
    Task<BranchOfficeAddress?> GetCurrentPrimaryAsync(long branchOfficeId, CancellationToken cancellationToken = default);
}





