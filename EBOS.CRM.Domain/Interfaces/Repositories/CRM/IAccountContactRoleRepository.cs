using EBOS.Core.Primitives.Interfaces;
using EBOS.CRM.Domain.Entities.CRM;

namespace EBOS.CRM.Domain.Interfaces.Repositories.CRM;

public interface IAccountContactRoleRepository : IPagedRepository<AccountContactRole>, IUnitOfWork
{
    Task<IReadOnlyCollection<AccountContactRole>> GetByAccountContactIdsAsync(long tenantId, IReadOnlyCollection<long> accountContactIds,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<AccountContactRole>> GetByAccountContactPagedAsync(long tenantId, long accountContactId,
        int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<int> CountByAccountContactAsync(long tenantId, long accountContactId,
        CancellationToken cancellationToken = default);
}
