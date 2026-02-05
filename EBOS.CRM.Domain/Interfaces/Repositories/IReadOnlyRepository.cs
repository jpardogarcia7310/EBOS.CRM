namespace EBOS.CRM.Domain.Interfaces.Repositories;

public interface IReadOnlyRepository<T> where T : class
{
    Task<T?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<T>> GetAllAsync(CancellationToken cancellationToken = default);
}
