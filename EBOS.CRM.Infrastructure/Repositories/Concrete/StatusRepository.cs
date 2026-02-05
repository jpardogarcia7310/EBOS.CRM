using EBOS.CRM.Domain.Entities;
using EBOS.CRM.Domain.Interfaces.Repositories;

namespace EBOS.CRM.Infrastructure.Repositories.Concrete;

public class StatusRepository(CrmDbContext context) : IStatusRepository
{
    private readonly DbSet<Status> _dbSet = context.Set<Status>();

    #region Commands
    public Task AddAsync(Status entity, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task AddRangeAsync(IEnumerable<Status> entities, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task AttachAsync(Status entity, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task UpdateAsync(Status entity, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task DeleteAsync(Status entity, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();
    #endregion

    #region Queries
    public Task<Status?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        => _dbSet.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

    public async Task<IReadOnlyCollection<Status>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Status>> GetAllPagedAsync(int pageNumber, int pageSize, 
        CancellationToken cancellationToken = default)
    {
        var safePageNumber = Math.Max(1, pageNumber);
        var safePageSize = Math.Max(1, pageSize);

        return await _dbSet.AsNoTracking()
            .OrderBy(s => s.Id)
            .Skip((safePageNumber - 1) * safePageSize)
            .Take(safePageSize)
            .ToListAsync(cancellationToken);
    }

    public Task<int> CountAsync(CancellationToken cancellationToken = default)
        => _dbSet.AsNoTracking().CountAsync(cancellationToken);

    public IQueryable<Status> AsQueryable(bool includeErased = false)
        => _dbSet.AsQueryable();
    #endregion
}
