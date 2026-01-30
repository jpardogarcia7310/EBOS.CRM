using EBOS.CRM.Domain.Entities;
using EBOS.CRM.Domain.Interfaces.Repositories;
using EBOS.CRM.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EBOS.CRM.Infrastructure.Repositories.Concrete;

public class IdentificationTypeRepository(CrmDbContext context) : IIdentificationTypeRepository
{
    private readonly DbSet<IdentificationType> _dbSet = context.Set<IdentificationType>();
    
    #region Queries
    public Task<IdentificationType?> GetByIdAsync(long id, CancellationToken cancellationToken = default) => 
        _dbSet.AsNoTracking().FirstOrDefaultAsync(it => it.Id == id, cancellationToken); 
    public Task<ICollection<IdentificationType>> GetAllAsync(CancellationToken cancellationToken = default) => 
        _dbSet.AsNoTracking().ToListAsync(cancellationToken) 
            .ContinueWith<ICollection<IdentificationType>>(t => t.Result, cancellationToken);
    #endregion
}