using EBOS.Core.Primitives.Interfaces;
using EBOS.CRM.Domain.Entities;

namespace EBOS.CRM.Domain.Interfaces.Repositories;

public interface ICountryRepository : IUnitOfWork
{
    #region Commands
    Task<Country> AddAsync(Country country, CancellationToken cancellationToken = default);
    Task UpdateAsync(Country country, CancellationToken cancellationToken = default);
    Task DeleteAsync(long id, CancellationToken cancellationToken = default);
    #endregion

    #region Queries
    Task<Country?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<ICollection<Country>> GetAllAsync(CancellationToken cancellationToken = default);
    #endregion
}