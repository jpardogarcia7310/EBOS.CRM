using EBOS.CRM.Domain.Entities.EBOS;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;
using Microsoft.EntityFrameworkCore;

namespace EBOS.CRM.Infrastructure.Repositories.Concrete.EBOS;

public class ChannelCountryRepository(CrmDbContext context) : IChannelCountryRepository
{
    private readonly DbSet<ChannelCountry> _dbSet = context.Set<ChannelCountry>();

    public Task<ChannelCountry?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        => _dbSet.AsNoTracking()
            .Include(x => x.ChannelType)
            .Include(x => x.Country)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public async Task<IReadOnlyCollection<ChannelCountry>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .Include(x => x.ChannelType)
            .Include(x => x.Country)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<ChannelCountry>> GetAllPagedAsync(int pageNumber, int pageSize,
        CancellationToken cancellationToken = default)
    {
        var safePageNumber = Math.Max(1, pageNumber);
        var safePageSize = Math.Max(1, pageSize);

        return await _dbSet.AsNoTracking()
            .Include(x => x.ChannelType)
            .Include(x => x.Country)
            .OrderBy(it => it.Id)
            .Skip((safePageNumber - 1) * safePageSize)
            .Take(safePageSize)
            .ToListAsync(cancellationToken);
    }

    public Task<int> CountAsync(CancellationToken cancellationToken = default)
        => _dbSet.AsNoTracking().CountAsync(cancellationToken);

    public IQueryable<ChannelCountry> AsQueryable(bool includeErased = false)
        => _dbSet.AsQueryable();

    public Task<bool> IsAllowedAsync(long channelTypeId, long countryId, CancellationToken cancellationToken)
    {
        return _dbSet.AsNoTracking()
            .AnyAsync(x => x.ChannelTypeId == channelTypeId
                           && x.CountryId == countryId
                           && x.IsActive, cancellationToken);
    }
}
