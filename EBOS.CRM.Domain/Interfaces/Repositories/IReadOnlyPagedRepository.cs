namespace EBOS.CRM.Domain.Interfaces.Repositories;

public interface IReadOnlyPagedRepository<T> : IReadOnlyRepository<T> where T : class
{
    Task<IReadOnlyCollection<T>> GetAllPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<int> CountAsync(CancellationToken cancellationToken = default);
}
