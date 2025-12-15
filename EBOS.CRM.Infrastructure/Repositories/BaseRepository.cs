using EBOS.Core.Primitives.Interfaces;
using EBOS.CRM.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace EBOS.CRM.Infrastructure.Repositories;

public class BaseRepository<T> : IUnitOfWork where T : class
{
    private readonly CrmDbContext _context;
    protected readonly DbSet<T> _dbSet;
    private IDbContextTransaction? _currentTransaction;

    public BaseRepository(CrmDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = _context.Set<T>();
    }

    #region Commands
    public virtual async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
    }

    public virtual Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Update(entity);
        return Task.CompletedTask;
    }

    public virtual Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
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

        return Task.CompletedTask;
    }
    #endregion

    #region Queries
    public virtual async Task<T?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var result = await _dbSet.FindAsync(new object[] { id }, cancellationToken);
        return result is null ? null : (T?)result;
    }

    public virtual async Task<ICollection<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking().ToListAsync(cancellationToken);
    }
    #endregion

    #region IUnitOfWork
    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction != null)
            return;

        _currentTransaction = await _context.Database.BeginTransactionAsync(cancellationToken);
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
        // Responder a la cancelación lo antes posible
        cancellationToken.ThrowIfCancellationRequested();

        if (_currentTransaction == null)
            return;

        // No existe DisposeAsync que acepte CancellationToken; comprobamos cancelación antes y después
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
        return await _context.SaveChangesAsync(cancellationToken);
    }
    #endregion
}