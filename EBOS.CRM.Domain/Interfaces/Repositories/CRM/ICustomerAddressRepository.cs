using EBOS.Core.Primitives.Interfaces;
using EBOS.CRM.Domain.Entities.CRM;

namespace EBOS.CRM.Domain.Interfaces.Repositories.CRM;

public interface ICustomerAddressRepository : IRepository<CustomerAddress>, IUnitOfWork
{
    Task<bool> ExistsPrimaryAddressForCustomerAsync(long customerId, CancellationToken cancellationToken = default);
    Task<CustomerAddress?> GetCurrentPrimaryAsync(long customerId, CancellationToken cancellationToken = default);
}