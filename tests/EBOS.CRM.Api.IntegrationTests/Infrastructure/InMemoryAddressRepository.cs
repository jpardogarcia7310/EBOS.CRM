using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Primitives.Paging;

namespace EBOS.CRM.Api.IntegrationTests.Infrastructure;

public sealed class InMemoryAddressRepository : IAddressRepository
{
    private readonly List<Address> _items = new();
    private long _nextId = 1;

    public IReadOnlyList<Address> Items => _items;

    public Task AddAsync(Address entity, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (entity.Id == 0)
        {
            entity.Id = _nextId++;
        }
        _items.Add(entity);
        return Task.CompletedTask;
    }

    public Task AddRangeAsync(IEnumerable<Address> entities, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        foreach (var entity in entities)
        {
            if (entity.Id == 0)
            {
                entity.Id = _nextId++;
            }
            _items.Add(entity);
        }
        return Task.CompletedTask;
    }

    public Task AttachAsync(Address entity, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!_items.Contains(entity))
        {
            _items.Add(entity);
        }
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Address entity, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var index = _items.FindIndex(x => x.Id == entity.Id);
        if (index >= 0)
        {
            _items[index] = entity;
        }
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Address entity, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _items.RemoveAll(x => x.Id == entity.Id);
        return Task.CompletedTask;
    }

    public Task<Address?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(_items.FirstOrDefault(x => x.Id == id));
    }

    public Task<ICollection<Address>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult<ICollection<Address>>(_items.ToList());
    }

    public Task<PagedResult<Address>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var normalized = query.Normalize();
        var totalCount = _items.Count;
        var pageSize = normalized.PageSize;
        var pageNumber = normalized.PageNumber;
        var totalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize);
        var items = _items
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return Task.FromResult(new PagedResult<Address>(
            items,
            pageNumber,
            pageSize,
            totalCount,
            totalPages,
            normalized.SortBy,
            normalized.SortDirection,
            normalized.Filter));
    }

    public Task BeginTransactionAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

    public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

    public Task EndTransactionAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

    public Task RollbackAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => Task.FromResult(1);

    public Task<bool> ExistPrimaryAddressInCustomerId(long customerId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var exists = _items.Any(x =>  x is { Erased: false });
        return Task.FromResult(exists);
    }
}
