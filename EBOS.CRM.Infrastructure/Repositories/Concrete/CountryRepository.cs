using EBOS.CRM.Domain.Entities;
using EBOS.CRM.Domain.Interfaces.Repositories;
using EBOS.CRM.Domain.Primitives.Paging;
using EBOS.CRM.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EBOS.CRM.Infrastructure.Repositories.Concrete;

public class CountryRepository(CrmDbContext context) : ICountryRepository
{
    private readonly DbSet<Country> _dbSet = context.Set<Country>();

    #region Queries
    public Task<Country?> GetByIdAsync(long id, CancellationToken cancellationToken = default) =>
        _dbSet.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    public Task<ICollection<Country>> GetAllAsync(CancellationToken cancellationToken = default) =>
        _dbSet.AsNoTracking().ToListAsync(cancellationToken)
            .ContinueWith<ICollection<Country>>(t => t.Result, cancellationToken);

    public Task<PagedResult<Country>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default)
        => _dbSet.AsNoTracking()
            .ApplyPagedQueryAsync(query, cancellationToken);
    #endregion
}
