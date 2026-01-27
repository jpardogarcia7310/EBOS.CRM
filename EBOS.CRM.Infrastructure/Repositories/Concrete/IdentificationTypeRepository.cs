using EBOS.CRM.Domain.Entities;
using EBOS.CRM.Domain.Interfaces.Repositories;
using EBOS.CRM.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace EBOS.CRM.Infrastructure.Repositories.Concrete;

public class IdentificationTypeRepository(CrmDbContext context) : IIdentificationTypeRepository
{
    private readonly DbSet<IdentificationType> _dbSet = context.Set<IdentificationType>();
    private IDbContextTransaction? _currentTransaction;
    
    #region Queries
    public async Task<IdentificationType?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<ICollection<IdentificationType>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking().ToListAsync(cancellationToken);
    }
    #endregion

    #region IUnitOfWork
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await context.SaveChangesAsync(cancellationToken);
    }

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
        // Respond to the cancellation as soon as possible
        cancellationToken.ThrowIfCancellationRequested();

        if (_currentTransaction == null)
            return;

        await _currentTransaction.DisposeAsync();
        _currentTransaction = null;
    }
    #endregion
}