using EBOS.CRM.Domain.Entities.EBOS;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;

namespace EBOS.CRM.Infrastructure.Repositories.Concrete.EBOS;

public class ChannelTypeRepository(CrmDbContext context) : IChannelTypeRepository
{
    private readonly DbSet<ChannelType> _dbSet = context.Set<ChannelType>();

    #region Queries
    public Task<ChannelType?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        => _dbSet.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public async Task<IReadOnlyCollection<ChannelType>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<ChannelType>> GetAllPagedAsync(int pageNumber, int pageSize,
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

    public IQueryable<ChannelType> AsQueryable(bool includeErased = false)
        => _dbSet.AsQueryable();
    #endregion
}
