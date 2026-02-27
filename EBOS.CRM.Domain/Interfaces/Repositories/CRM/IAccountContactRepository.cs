using EBOS.Core.Primitives.Interfaces;
using EBOS.CRM.Domain.Entities.CRM;

namespace EBOS.CRM.Domain.Interfaces.Repositories.CRM;

public interface IAccountContactRepository : IPagedRepository<AccountContact>, IUnitOfWork
{
    Task<IReadOnlyCollection<AccountContact>> GetByCustomerIdsAsync(long tenantId, IReadOnlyCollection<long> customerIds,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<AccountContact>> GetByCorporateCustomerPagedAsync(long tenantId, long corporateCustomerId,
        int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<int> CountByCorporateCustomerAsync(long tenantId, long corporateCustomerId,
        CancellationToken cancellationToken = default);
}
