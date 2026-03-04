using System.Threading;
using System.Threading.Tasks;
using EBOS.Core.Primitives.Interfaces;
using EBOS.CRM.Domain.Entities.CRM;

namespace EBOS.CRM.Domain.Interfaces.Repositories.CRM;

public interface ICustomerAddressRepository : IPagedRepository<CustomerAddress>, IUnitOfWork
{
    Task<IReadOnlyCollection<CustomerAddress>> GetByCustomerIdsAsync(long tenantId, IReadOnlyCollection<long> customerIds,
        CancellationToken cancellationToken = default);
    Task<bool> ExistsPrimaryAddressForCustomerAsync(long customerId, CancellationToken cancellationToken = default);
    Task<CustomerAddress?> GetCurrentPrimaryAsync(long customerId, CancellationToken cancellationToken = default);
}






