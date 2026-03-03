using EBOS.CRM.Domain.Entities.CRM;
using EBOS.Core.Primitives.Interfaces;

namespace EBOS.CRM.Domain.Interfaces.Repositories.CRM;

public interface ICustomerPrivacyRequestRepository : IPagedRepository<CustomerPrivacyRequest>, IUnitOfWork
{
    Task<IReadOnlyCollection<CustomerPrivacyRequest>> GetByCustomerPagedAsync(long tenantId, long customerId,
        int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<int> CountByCustomerAsync(long tenantId, long customerId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<CustomerPrivacyRequest>> GetByStatusPagedAsync(long tenantId, string status,
        int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<int> CountByStatusAsync(long tenantId, string status, CancellationToken cancellationToken = default);
    Task<CustomerPrivacyRequest?> GetActiveByCustomerAndTypeAsync(long tenantId, long customerId, string requestType,
        CancellationToken cancellationToken = default);
}
