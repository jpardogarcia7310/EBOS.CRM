using EBOS.Core.Primitives.Interfaces;
using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories;

namespace EBOS.CRM.Domain.Interfaces.Repositories.CRM;

public interface IBranchOfficeAddressRepository : IRepository<BranchOfficeAddress>, IPagedRepository<BranchOfficeAddress>, IUnitOfWork
{
    Task<BranchOfficeAddress?> GetCurrentPrimaryAsync(long branchOfficeId, CancellationToken cancellationToken = default);
}
