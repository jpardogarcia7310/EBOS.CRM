using EBOS.Core.Primitives.Interfaces;
using EBOS.CRM.Domain.Entities;

namespace EBOS.CRM.Domain.Interfaces.Repositories;

public interface ICountryRepository
{
    #region Queries
    Task<Country?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<ICollection<Country>> GetAllAsync(CancellationToken cancellationToken = default);
    #endregion
}