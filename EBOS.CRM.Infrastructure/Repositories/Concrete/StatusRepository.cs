using EBOS.CRM.Domain.Entities;
using EBOS.CRM.Domain.Interfaces.Repositories;
using EBOS.CRM.Infrastructure.Persistence;


namespace EBOS.CRM.Infrastructure.Repositories.Concrete;

public class StatusRepository(CrmDbContext context) : IStatusRepository
{
    private readonly DbSet<Status> _dbSet = context.Set<Status>();

    #region Queries
    public Task<Status?> GetByIdAsync(long id, CancellationToken cancellationToken = default) =>
        _dbSet.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    public Task<ICollection<Status>> GetAllAsync(CancellationToken cancellationToken = default) =>
        _dbSet.AsNoTracking().ToListAsync(cancellationToken)
            .ContinueWith<ICollection<Status>>(t => t.Result, cancellationToken);

    #endregion
}




