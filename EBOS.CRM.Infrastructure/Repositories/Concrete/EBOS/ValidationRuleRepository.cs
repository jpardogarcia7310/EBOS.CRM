using EBOS.CRM.Domain.Entities.EBOS;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;

namespace EBOS.CRM.Infrastructure.Repositories.Concrete.EBOS;

public class ValidationRuleRepository(CrmDbContext context) : IValidationRuleRepository
{
    private readonly DbSet<ValidationRule> _dbSet = context.Set<ValidationRule>();

    public Task<ValidationRule?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        => _dbSet.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

    public async Task<IReadOnlyCollection<ValidationRule>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<ValidationRule>> GetAllPagedAsync(int pageNumber, int pageSize,
        CancellationToken cancellationToken = default)
    {
        var safePageNumber = Math.Max(1, pageNumber);
        var safePageSize = Math.Max(1, pageSize);

        return await _dbSet.AsNoTracking()
            .OrderBy(r => r.Id)
            .Skip((safePageNumber - 1) * safePageSize)
            .Take(safePageSize)
            .ToListAsync(cancellationToken);
    }

    public Task<int> CountAsync(CancellationToken cancellationToken = default)
        => _dbSet.AsNoTracking().CountAsync(cancellationToken);

    public IQueryable<ValidationRule> AsQueryable(bool includeErased = false)
        => includeErased ? _dbSet.AsQueryable() : _dbSet.AsQueryable().Where(r => !r.Erased);

    public async Task<IReadOnlyCollection<ValidationRule>> GetByKeysAsync(IReadOnlyCollection<string> keys,
        CancellationToken cancellationToken = default)
    {
        if (keys.Count == 0)
        {
            return Array.Empty<ValidationRule>();
        }

        return await _dbSet.AsNoTracking()
            .Where(r => !r.Erased && r.IsActive && keys.Contains(r.Key))
            .ToListAsync(cancellationToken);
    }
}
