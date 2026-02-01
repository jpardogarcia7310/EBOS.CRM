using EBOS.CRM.Domain.Entities;

namespace EBOS.CRM.Domain.Interfaces.Repositories;

public interface IAddressTypeRepository : IPagedRepository<AddressType>
{
    #region Queries
    Task<AddressType?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<ICollection<AddressType>> GetAllAsync(CancellationToken cancellationToken = default);
    #endregion
}
