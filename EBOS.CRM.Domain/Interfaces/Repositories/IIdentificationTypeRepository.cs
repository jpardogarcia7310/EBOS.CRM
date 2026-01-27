using EBOS.CRM.Domain.Entities;

namespace EBOS.CRM.Domain.Interfaces.Repositories;

public interface IIdentificationTypeRepository
{
    #region Queries
    Task<IdentificationType?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<ICollection<IdentificationType>> GetAllAsync(CancellationToken cancellationToken = default);
    #endregion
}