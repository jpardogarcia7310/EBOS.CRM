using EBOS.Core.Primitives.Interfaces;
using EBOS.CRM.Domain.Entities;


namespace EBOS.CRM.Domain.Interfaces.Repositories;

public interface IStatusRepository
{
    #region Queries
    Task<Status?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<ICollection<Status>> GetAllAsync(CancellationToken cancellationToken = default);
    #endregion
}





