using EBOS.CRM.Domain.Entities;
using EBOS.CRM.Domain.Interfaces.Repositories;

namespace EBOS.CRM.Infrastructure.Repositories.Concrete;

public class IdentificationTypeRepository(CrmDbContext context) : IIdentificationTypeRepository
{
    private readonly DbSet<IdentificationType> _dbSet = context.Set<IdentificationType>();
    
    #region Commands
    public Task AddAsync(IdentificationType entity, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task AddRangeAsync(IEnumerable<IdentificationType> entities, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task AttachAsync(IdentificationType entity, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task UpdateAsync(IdentificationType entity, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task DeleteAsync(IdentificationType entity, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();
    #endregion

    #region Queries
    public Task<IdentificationType?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        => _dbSet.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public async Task<IReadOnlyCollection<IdentificationType>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<IdentificationType>> GetAllPagedAsync(int pageNumber, int pageSize, 
        CancellationToken cancellationToken = default)
    {
        var safePageNumber = Math.Max(1, pageNumber);
        var safePageSize = Math.Max(1, pageSize);

        return await _dbSet.AsNoTracking()
            .OrderBy(it => it.Id)
            .Skip((safePageNumber - 1) * safePageSize)
            .Take(safePageSize)
            .ToListAsync(cancellationToken);
    }

    public Task<int> CountAsync(CancellationToken cancellationToken = default)
        => _dbSet.AsNoTracking().CountAsync(cancellationToken);

    public IQueryable<IdentificationType> AsQueryable(bool includeErased = false)
        => _dbSet.AsQueryable();
    #endregion
}
