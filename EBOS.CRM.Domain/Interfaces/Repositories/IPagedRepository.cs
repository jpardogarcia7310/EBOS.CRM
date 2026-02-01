using EBOS.CRM.Domain.Primitives.Paging;

namespace EBOS.CRM.Domain.Interfaces.Repositories;

public interface IPagedRepository<TEntity> where TEntity : class
{
    Task<PagedResult<TEntity>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
}
