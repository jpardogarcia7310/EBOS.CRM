using EBOS.Core.Primitives.Interfaces;
using EBOS.CRM.Domain.Entities.CRM;

namespace EBOS.CRM.Domain.Interfaces.Repositories.CRM;

public interface IAccountHierarchyRepository : IPagedRepository<AccountHierarchy>, IUnitOfWork
{
    Task<IReadOnlyCollection<AccountHierarchy>> GetByAccountPagedAsync(long tenantId, long corporateCustomerId,
        int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<long>> GetParentIdsByChildIdsAsync(long tenantId, IReadOnlyCollection<long> childCorporateCustomerIds,
        CancellationToken cancellationToken = default);
    Task<int> CountByAccountAsync(long tenantId, long corporateCustomerId, CancellationToken cancellationToken = default);
}
