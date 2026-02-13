using EBOS.Core.Primitives.Interfaces;
using EBOS.CRM.Domain.Entities.CRM;

namespace EBOS.CRM.Domain.Interfaces.Repositories.CRM;

public interface ICustomerPreferenceRepository : IPagedRepository<CustomerPreference>, IUnitOfWork
{
    Task<IReadOnlyCollection<CustomerPreference>> GetByCustomerPagedAsync(long tenantId, long customerId,
        int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<int> CountByCustomerAsync(long tenantId, long customerId, CancellationToken cancellationToken = default);
}
