using EBOS.Core.Primitives.Interfaces;

namespace EBOS.CRM.Infrastructure.Repositories;

public class BaseRepository<T>(CrmDbContext context) : IPagedRepository<T> where T : class, ISoftDeletable
{
    protected readonly CrmDbContext Context = context;
    protected readonly DbSet<T> DbSet = context.Set<T>();
    private IDbContextTransaction? _currentTransaction;

    #region Commands
    public virtual Task AddAsync(T entity, CancellationToken cancellationToken = default)
        => DbSet.AddAsync(entity, cancellationToken).AsTask();

    public virtual Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        => DbSet.AddRangeAsync(entities, cancellationToken);

    public virtual Task AttachAsync(T entity, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Context.Attach(entity);
        return Task.CompletedTask;
    }

    public virtual Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        DbSet.Update(entity);
        return Task.CompletedTask;
    }

    public virtual Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        entity.Erased = true;
        DbSet.Update(entity);
        return Task.CompletedTask;
    }
    #endregion

    #region Queries
    public virtual async Task<T?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await DbSet.FindAsync([id], cancellationToken);
        return entity is { Erased: false } ? entity : null;
    }

    public virtual async Task<ICollection<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.AsNoTracking()
            .Where(e => !e.Erased)
            .ToListAsync(cancellationToken);
    }

    public virtual async Task<IReadOnlyCollection<T>> GetAllPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var safePageNumber = Math.Max(1, pageNumber);
        var safePageSize = Math.Max(1, pageSize);

        return await AsQueryable()
            .OrderBy(e => EF.Property<long>(e, "Id"))
            .Skip((safePageNumber - 1) * safePageSize)
            .Take(safePageSize)
            .ToListAsync(cancellationToken);
    }

    public virtual Task<int> CountAsync(CancellationToken cancellationToken = default)
        => DbSet.AsNoTracking()
            .Where(e => !e.Erased)
            .CountAsync(cancellationToken);

    public virtual IQueryable<T> AsQueryable(bool includeErased = false)
        => includeErased ? DbSet.AsQueryable() : DbSet.Where(e => !e.Erased);
    #endregion

    #region IUnitOfWork
    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction != null)
            return;

        _currentTransaction = await Context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction == null)
            return;

        await SaveChangesAsync(cancellationToken);
        await _currentTransaction.CommitAsync(cancellationToken);
        await _currentTransaction.DisposeAsync();
        _currentTransaction = null;
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction == null)
            return;

        await _currentTransaction.RollbackAsync(cancellationToken);
        await _currentTransaction.DisposeAsync();
        _currentTransaction = null;
    }

    public async Task EndTransactionAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (_currentTransaction == null)
            return;

        await _currentTransaction.DisposeAsync();
        _currentTransaction = null;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => Context.SaveChangesAsync(cancellationToken);
    #endregion
}





