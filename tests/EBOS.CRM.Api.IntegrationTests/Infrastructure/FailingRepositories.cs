using EBOS.CRM.Domain.Entities;
using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;

namespace EBOS.CRM.Api.IntegrationTests.Infrastructure;

public sealed class FailingAddressTypeRepository : IAddressTypeRepository
{
    public Task<AddressType?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task<ICollection<AddressType>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }
}

public sealed class FailingIdentificationTypeRepository : IIdentificationTypeRepository
{
    public Task<IdentificationType?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task<ICollection<IdentificationType>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }
}

public sealed class FailingAddressRepository : IAddressRepository
{
    public Task AddAsync(Address entity, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task AddRangeAsync(IEnumerable<Address> entities, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public void AttachAsync(Address entity, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public void UpdateAsync(Address entity, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public void DeleteAsync(Address entity, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task<Address?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task<ICollection<Address>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task CommitAsync(CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task EndTransactionAsync(CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task<bool> ExistPrimaryAddressInCustomerId(long customerId, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }
}
