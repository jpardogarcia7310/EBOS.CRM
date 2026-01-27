using EBOS.Core.Primitives.Interfaces;
using EBOS.CRM.Domain.Entities.CRM;

namespace EBOS.CRM.Domain.Interfaces.Repositories.CRM;

public interface IAddressRepository : IRepository<Address>, IUnitOfWork
{
    Task<bool> ExistPrimaryAddressInCustomerId(long customerId, CancellationToken cancellationToken = default);
}
