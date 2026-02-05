using EBOS.CRM.Domain.Entities;
using EBOS.CRM.Domain.Interfaces.Repositories;

namespace EBOS.CRM.Infrastructure.Repositories.Concrete;

public class CountryRepository(CrmDbContext context) : ICountryRepository
{
    private readonly DbSet<Country> _dbSet = context.Set<Country>();

    #region Commands
    public Task AddAsync(Country entity, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task AddRangeAsync(IEnumerable<Country> entities, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task AttachAsync(Country entity, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task UpdateAsync(Country entity, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task DeleteAsync(Country entity, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();
    #endregion

    #region Queries
    public Task<Country?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        => _dbSet.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public async Task<IReadOnlyCollection<Country>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Country>> GetAllPagedAsync(int pageNumber, int pageSize, 
        CancellationToken cancellationToken = default)
    {
        var safePageNumber = Math.Max(1, pageNumber);
        var safePageSize = Math.Max(1, pageSize);

        return await _dbSet.AsNoTracking()
            .OrderBy(c => c.Id)
            .Skip((safePageNumber - 1) * safePageSize)
            .Take(safePageSize)
            .ToListAsync(cancellationToken);
    }

    public Task<int> CountAsync(CancellationToken cancellationToken = default)
        => _dbSet.AsNoTracking().CountAsync(cancellationToken);

    public IQueryable<Country> AsQueryable(bool includeErased = false)
        => _dbSet.AsQueryable();
    #endregion
}
