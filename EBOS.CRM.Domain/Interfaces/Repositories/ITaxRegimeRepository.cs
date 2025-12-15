using EBOS.Core.Primitives.Interfaces;
using EBOS.CRM.Domain.Entities;

namespace EBOS.CRM.Domain.Interfaces.Repositories;

public interface ITaxRegimeRepository : IUnitOfWork
{
    #region Queries
    Task<TaxRegime?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<ICollection<TaxRegime>> GetAllAsync(CancellationToken cancellationToken = default);
    #endregion
}