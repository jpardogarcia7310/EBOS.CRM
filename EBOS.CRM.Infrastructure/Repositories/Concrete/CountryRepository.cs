using EBOS.CRM.Domain.Entities;
using EBOS.CRM.Domain.Interfaces.Repositories;
using EBOS.CRM.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace EBOS.CRM.Infrastructure.Repositories.Concrete;

public class CountryRepository(CrmDbContext context) : ICountryRepository
{

    private readonly DbSet<Country> _dbSet = context.Set<Country>();
    private IDbContextTransaction? _currentTransaction;

    #region Commands
    public async Task<Country> AddAsync(Country country, CancellationToken cancellationToken = default)
    {
        var entry = await _dbSet.AddAsync(country, cancellationToken);
        return entry.Entity;
    }
    public async Task UpdateAsync(Country country, CancellationToken cancellationToken = default)
    {
        // Si usas AsNoTracking en GetByIdAsync, necesitas Attach o Update
        _dbSet.Update(country);
    }

    public async Task DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbSet.FindAsync(new object[] { id }, cancellationToken);
        if (entity is null)
        {
            return; 
        }
        _dbSet.Remove(entity);
    }    
    #endregion

    #region Queries
    public async Task<Country?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(c => c.Id == id, cancellationToken: cancellationToken);
    }
    public async Task<ICollection<Country>> GetAllAsync(CancellationToken cancellationToken = default)
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
    public async Task EndTransactionAsync()
    {
        if (_currentTransaction == null)
            return;
        await _currentTransaction.DisposeAsync();
        _currentTransaction = null;
    }
    #endregion
}