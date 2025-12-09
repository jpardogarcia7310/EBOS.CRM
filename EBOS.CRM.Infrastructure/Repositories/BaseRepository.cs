using EBOS.Core.Primitives.Interfaces;
using EBOS.CRM.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace EBOS.CRM.Infrastructure.Repositories;

public class BaseRepository<T>(CrmDbContext context) : IUnitOfWork where T : class
{
    protected readonly DbSet<T> _dbSet = context.Set<T>();
    private IDbContextTransaction? _currentTransaction;

    #region Commands
    public virtual async Task AddAsync(T entity, CancellationToken cancellationToken)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
    }
    public virtual void Update(T entity)
    {
        _dbSet.Update(entity);
    }
    public virtual void Delete(T entity)
    {
        if (entity is ISoftDeletable softDeletable)
        {
            softDeletable.Erased = true;
            _dbSet.Update(entity);
        }
        else
        {
            _dbSet.Remove(entity);
        }
    }
    #endregion

    #region Queries
    public virtual async Task<T?> GetByIdAsync(long id)
    {
        return await _dbSet.FindAsync(id);
    }
    public virtual async Task<ICollection<T>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _dbSet.ToListAsync(cancellationToken);
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

    public async Task EndTransactionAsync()
    {
        if (_currentTransaction == null)
            return;
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
