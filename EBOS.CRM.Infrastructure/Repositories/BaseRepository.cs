using EBOS.Core.Primitives.Interfaces;
using EBOS.CRM.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace EBOS.CRM.Infrastructure.Repositories;

public class BaseRepository<T>(CrmDbContext context) where T : class, ISoftDeletable
{
    protected readonly DbSet<T> DbSet = context.Set<T>();
    private IDbContextTransaction? _currentTransaction;

    #region Commands
    public virtual async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await DbSet.AddAsync(entity, cancellationToken);
    }
    
    public virtual async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        await DbSet.AddRangeAsync(entities, cancellationToken);
    }

    public virtual void AttachAsync(T entity, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        context.Attach(entity);
    }
    
    public virtual void UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        DbSet.Update(entity);
    }

    public virtual void DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (entity is ISoftDeletable softDeletable)
        {
            softDeletable.Erased = true;
            DbSet.Update(entity);
        }
        else
        {
            DbSet.Remove(entity);
        }
    }
    #endregion

    #region Queries
    public virtual async Task<T?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var result = await DbSet.FindAsync([id], cancellationToken);
        return result;
    }

    public virtual async Task<ICollection<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.AsNoTracking().ToListAsync(cancellationToken);
    }
    #endregion

    #region IUnitOfWork
    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction != null)
            return;

        _currentTransaction = await context.Database.BeginTransactionAsync(cancellationToken);
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

    public async Task EndTransactionAsync(CancellationToken cancellationToken = default)
    {
        // Respond to the cancellation as soon as possible
        cancellationToken.ThrowIfCancellationRequested();

        if (_currentTransaction == null)
            return;

        // There is no DisposeAsync that accepts CancellationToken; we check for cancellation before and after
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

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await context.SaveChangesAsync(cancellationToken);
    }
    #endregion
}