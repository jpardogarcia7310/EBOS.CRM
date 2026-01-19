using EBOS.Core.Primitives.Interfaces;
using EBOS.CRM.Domain.Entities;

namespace EBOS.CRM.Domain.Interfaces.Repositories;

public interface IPaisRepository : IUnitOfWork
{
    #region Queries
    Task<Pais?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<ICollection<Pais>> GetAllAsync(CancellationToken cancellationToken = default);
    #endregion
}