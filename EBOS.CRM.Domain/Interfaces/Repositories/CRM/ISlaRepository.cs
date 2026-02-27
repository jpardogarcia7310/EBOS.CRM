using EBOS.Core.Primitives.Interfaces;
using EBOS.CRM.Domain.Entities.CRM;

namespace EBOS.CRM.Domain.Interfaces.Repositories.CRM;

public interface ISlaRepository : IPagedRepository<Sla>, IUnitOfWork
{
    Task<IReadOnlyCollection<Sla>> GetByIdsAsync(IReadOnlyCollection<long> ids, CancellationToken cancellationToken = default);


}
