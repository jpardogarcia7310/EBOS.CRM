using EBOS.CRM.Domain.Entities;
using EBOS.CRM.Domain.Interfaces.Repositories;
using EBOS.CRM.Domain.Primitives.Paging;
using EBOS.CRM.Infrastructure.Repositories;
using EBOS.CRM.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EBOS.CRM.Infrastructure.Repositories.Concrete;

public class AddressTypeRepository(CrmDbContext context) : IAddressTypeRepository
{
    private readonly DbSet<AddressType> _dbSet = context.Set<AddressType>();

    #region Queries
    public Task<AddressType?> GetByIdAsync(long id, CancellationToken cancellationToken = default) => 
        _dbSet.AsNoTracking().FirstOrDefaultAsync(at => at.Id == id, cancellationToken); 
    public Task<ICollection<AddressType>> GetAllAsync(CancellationToken cancellationToken = default) => 
        _dbSet.AsNoTracking().ToListAsync(cancellationToken) 
            .ContinueWith<ICollection<AddressType>>(t => t.Result, cancellationToken);

    public Task<PagedResult<AddressType>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default)
        => _dbSet.AsNoTracking()
            .ApplyPagedQueryAsync(query, cancellationToken);
    #endregion
}
