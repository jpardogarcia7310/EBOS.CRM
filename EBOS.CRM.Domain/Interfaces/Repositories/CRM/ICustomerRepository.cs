using EBOS.Core.Primitives.Interfaces;
using EBOS.CRM.Domain.Entities.CRM;

namespace EBOS.CRM.Domain.Interfaces.Repositories.CRM;

public interface ICustomerRepository : IRepository<Customer>, IUnitOfWork
{
    Task<Customer?> GetWithAddressesAsync(long id, CancellationToken cancellationToken = default);
}