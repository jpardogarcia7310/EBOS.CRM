using EBOS.Core.Primitives.Interfaces;
using EBOS.CRM.Domain.Entities;

namespace EBOS.CRM.Domain.Interfaces.Repositories;

public interface IEstadoRepository : IUnitOfWork
{
    #region Queries
    Task<Estado?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<ICollection<Estado>> GetAllAsync(CancellationToken cancellationToken = default);
    #endregion
}