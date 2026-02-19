using EBOS.Core.Primitives.Interfaces;
using EBOS.CRM.Domain.Entities.CRM;

namespace EBOS.CRM.Domain.Interfaces.Repositories.CRM;

public interface ICustomerConsentRepository : IPagedRepository<CustomerConsent>, IUnitOfWork
{
    Task<IReadOnlyCollection<CustomerConsent>> GetByCustomerIdsAsync(long tenantId, IReadOnlyCollection<long> customerIds,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<CustomerConsent>> GetByCustomerPagedAsync(long tenantId, long customerId,
        int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<int> CountByCustomerAsync(long tenantId, long customerId, CancellationToken cancellationToken = default);
}
