using EBOS.CRM.Domain.Entities;
using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;

namespace EBOS.CRM.IntegrationTests.Infrastructure;

public sealed class FailingAddressTypeRepository : IAddressTypeRepository
{
    public Task AddAsync(AddressType entity, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task AddRangeAsync(IEnumerable<AddressType> entities, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task AttachAsync(AddressType entity, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task UpdateAsync(AddressType entity, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task DeleteAsync(AddressType entity, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }
    public Task<AddressType?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task<IReadOnlyCollection<AddressType>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }
    public Task<IReadOnlyCollection<AddressType>> GetAllPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public IQueryable<AddressType> AsQueryable(bool includeErased = false)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }
}

public sealed class FailingIdentificationTypeRepository : IIdentificationTypeRepository
{
    public Task AddAsync(IdentificationType entity, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task AddRangeAsync(IEnumerable<IdentificationType> entities, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task AttachAsync(IdentificationType entity, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task UpdateAsync(IdentificationType entity, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task DeleteAsync(IdentificationType entity, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }
    public Task<IdentificationType?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task<IReadOnlyCollection<IdentificationType>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }
    public Task<IReadOnlyCollection<IdentificationType>> GetAllPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public IQueryable<IdentificationType> AsQueryable(bool includeErased = false)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }
}

public sealed class FailingCountryRepository : ICountryRepository
{
    public Task AddAsync(Country entity, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task AddRangeAsync(IEnumerable<Country> entities, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task AttachAsync(Country entity, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task UpdateAsync(Country entity, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task DeleteAsync(Country entity, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }
    public Task<Country?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task<IReadOnlyCollection<Country>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }
    public Task<IReadOnlyCollection<Country>> GetAllPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public IQueryable<Country> AsQueryable(bool includeErased = false)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }
}

public sealed class FailingStatusRepository : IStatusRepository
{
    public Task AddAsync(Status entity, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task AddRangeAsync(IEnumerable<Status> entities, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task AttachAsync(Status entity, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task UpdateAsync(Status entity, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task DeleteAsync(Status entity, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }
    public Task<Status?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task<IReadOnlyCollection<Status>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }
    public Task<IReadOnlyCollection<Status>> GetAllPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public IQueryable<Status> AsQueryable(bool includeErased = false)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }
}

public sealed class FailingBankInformationRepository : IBankInformationRepository
{
    public Task AddAsync(BankInformation entity, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task AddRangeAsync(IEnumerable<BankInformation> entities, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task AttachAsync(BankInformation entity, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task UpdateAsync(BankInformation entity, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task DeleteAsync(BankInformation entity, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task<BankInformation?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task<IReadOnlyCollection<BankInformation>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }
    public Task<IReadOnlyCollection<BankInformation>> GetAllPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task<int> CountAsync(CancellationToken cancellationToken = default)
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
}

public sealed class FailingBranchOfficeRepository : IBranchOfficeRepository
{
    public Task AddAsync(BranchOffice entity, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task AddRangeAsync(IEnumerable<BranchOffice> entities, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task AttachAsync(BranchOffice entity, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task UpdateAsync(BranchOffice entity, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task DeleteAsync(BranchOffice entity, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task<BranchOffice?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task<IReadOnlyCollection<BranchOffice>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }
    public Task<IReadOnlyCollection<BranchOffice>> GetAllPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task<int> CountAsync(CancellationToken cancellationToken = default)
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
}

public sealed class FailingBranchOfficeAddressRepository : IBranchOfficeAddressRepository
{
    public Task AddAsync(BranchOfficeAddress entity, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task AddRangeAsync(IEnumerable<BranchOfficeAddress> entities, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task AttachAsync(BranchOfficeAddress entity, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task UpdateAsync(BranchOfficeAddress entity, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task DeleteAsync(BranchOfficeAddress entity, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task<BranchOfficeAddress?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task<IReadOnlyCollection<BranchOfficeAddress>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }
    public Task<IReadOnlyCollection<BranchOfficeAddress>> GetAllPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }
    public Task<BranchOfficeAddress?> GetCurrentPrimaryAsync(long branchOfficeId, CancellationToken cancellationToken = default)
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
}

public sealed class FailingCorporateCustomerRepository : ICorporateCustomerRepository
{
    public Task AddAsync(CorporateCustomer entity, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task AddRangeAsync(IEnumerable<CorporateCustomer> entities, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task AttachAsync(CorporateCustomer entity, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task UpdateAsync(CorporateCustomer entity, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task DeleteAsync(CorporateCustomer entity, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task<CorporateCustomer?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task<IReadOnlyCollection<CorporateCustomer>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }
    public Task<IReadOnlyCollection<CorporateCustomer>> GetAllPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task<int> CountAsync(CancellationToken cancellationToken = default)
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
}

public sealed class FailingCreditAccountRepository : ICreditAccountRepository
{
    public Task AddAsync(CreditAccount entity, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task AddRangeAsync(IEnumerable<CreditAccount> entities, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task AttachAsync(CreditAccount entity, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task UpdateAsync(CreditAccount entity, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task DeleteAsync(CreditAccount entity, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task<CreditAccount?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task<IReadOnlyCollection<CreditAccount>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }
    public Task<IReadOnlyCollection<CreditAccount>> GetAllPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task<int> CountAsync(CancellationToken cancellationToken = default)
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
}

public sealed class FailingCreditTransactionRepository : ICreditTransactionRepository
{
    public Task AddAsync(CreditTransaction entity, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task AddRangeAsync(IEnumerable<CreditTransaction> entities, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task AttachAsync(CreditTransaction entity, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task UpdateAsync(CreditTransaction entity, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task DeleteAsync(CreditTransaction entity, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task<CreditTransaction?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task<IReadOnlyCollection<CreditTransaction>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }
    public Task<IReadOnlyCollection<CreditTransaction>> GetAllPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task<int> CountAsync(CancellationToken cancellationToken = default)
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
}

public sealed class FailingCustomerRepository : ICustomerRepository
{
    public Task AddAsync(Customer entity, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task AddRangeAsync(IEnumerable<Customer> entities, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task AttachAsync(Customer entity, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task UpdateAsync(Customer entity, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task DeleteAsync(Customer entity, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task<Customer?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task<IReadOnlyCollection<Customer>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }
    public Task<IReadOnlyCollection<Customer>> GetAllPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }
    public Task<Customer?> GetWithAddressesAsync(long id, CancellationToken cancellationToken = default)
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
}

public sealed class FailingCustomerAddressRepository : ICustomerAddressRepository
{
    public Task AddAsync(CustomerAddress entity, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task AddRangeAsync(IEnumerable<CustomerAddress> entities, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task AttachAsync(CustomerAddress entity, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task UpdateAsync(CustomerAddress entity, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task DeleteAsync(CustomerAddress entity, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task<CustomerAddress?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task<IReadOnlyCollection<CustomerAddress>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }
    public Task<IReadOnlyCollection<CustomerAddress>> GetAllPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }
    public Task<bool> ExistsPrimaryAddressForCustomerAsync(long customerId, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task<CustomerAddress?> GetCurrentPrimaryAsync(long customerId, CancellationToken cancellationToken = default)
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
}

public sealed class FailingIndividualCustomerRepository : IIndividualCustomerRepository
{
    public Task AddAsync(IndividualCustomer entity, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task AddRangeAsync(IEnumerable<IndividualCustomer> entities, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task AttachAsync(IndividualCustomer entity, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task UpdateAsync(IndividualCustomer entity, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task DeleteAsync(IndividualCustomer entity, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task<IndividualCustomer?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task<IReadOnlyCollection<IndividualCustomer>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }
    public Task<IReadOnlyCollection<IndividualCustomer>> GetAllPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task<int> CountAsync(CancellationToken cancellationToken = default)
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
}

public sealed class FailingTaxInformationRepository : ITaxInformationRepository
{
    public Task AddAsync(TaxInformation entity, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task AddRangeAsync(IEnumerable<TaxInformation> entities, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task AttachAsync(TaxInformation entity, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task UpdateAsync(TaxInformation entity, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task DeleteAsync(TaxInformation entity, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task<TaxInformation?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task<IReadOnlyCollection<TaxInformation>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }
    public Task<IReadOnlyCollection<TaxInformation>> GetAllPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task<int> CountAsync(CancellationToken cancellationToken = default)
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
}

public sealed class FailingTaxInformationAddressRepository : ITaxInformationAddressRepository
{
    public Task AddAsync(TaxInformationAddress entity, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task AddRangeAsync(IEnumerable<TaxInformationAddress> entities, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task AttachAsync(TaxInformationAddress entity, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task UpdateAsync(TaxInformationAddress entity, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task DeleteAsync(TaxInformationAddress entity, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task<TaxInformationAddress?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task<IReadOnlyCollection<TaxInformationAddress>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }
    public Task<IReadOnlyCollection<TaxInformationAddress>> GetAllPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }
    public Task<TaxInformationAddress?> GetCurrentPrimaryAsync(long taxInformationId, CancellationToken cancellationToken = default)
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

    public Task AttachAsync(Address entity, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task UpdateAsync(Address entity, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task DeleteAsync(Address entity, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task<Address?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task<IReadOnlyCollection<Address>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }
    public Task<IReadOnlyCollection<Address>> GetAllPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task<int> CountAsync(CancellationToken cancellationToken = default)
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

public sealed class FailingLeadRepository : ILeadRepository
{
    public Task AddAsync(Lead entity, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task AddRangeAsync(IEnumerable<Lead> entities, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task AttachAsync(Lead entity, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task UpdateAsync(Lead entity, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task DeleteAsync(Lead entity, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task<Lead?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task<IReadOnlyCollection<Lead>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task<IReadOnlyCollection<Lead>> GetAllPagedAsync(int pageNumber, int pageSize,
        CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public IQueryable<Lead> AsQueryable(bool includeErased = false)
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
}

public sealed class FailingOpportunityRepository : IOpportunityRepository
{
    public Task AddAsync(Opportunity entity, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task AddRangeAsync(IEnumerable<Opportunity> entities, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task AttachAsync(Opportunity entity, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task UpdateAsync(Opportunity entity, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task DeleteAsync(Opportunity entity, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task<Opportunity?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task<IReadOnlyCollection<Opportunity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task<IReadOnlyCollection<Opportunity>> GetAllPagedAsync(int pageNumber, int pageSize,
        CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public IQueryable<Opportunity> AsQueryable(bool includeErased = false)
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
}

public sealed class FailingOpportunityStageRepository : IOpportunityStageRepository
{
    public Task AddAsync(OpportunityStage entity, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task AddRangeAsync(IEnumerable<OpportunityStage> entities, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task AttachAsync(OpportunityStage entity, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task UpdateAsync(OpportunityStage entity, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task DeleteAsync(OpportunityStage entity, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task<OpportunityStage?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task<IReadOnlyCollection<OpportunityStage>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task<IReadOnlyCollection<OpportunityStage>> GetAllPagedAsync(int pageNumber, int pageSize,
        CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public IQueryable<OpportunityStage> AsQueryable(bool includeErased = false)
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
}

public sealed class FailingQuoteRepository : IQuoteRepository
{
    public Task AddAsync(Quote entity, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task AddRangeAsync(IEnumerable<Quote> entities, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task AttachAsync(Quote entity, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task UpdateAsync(Quote entity, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task DeleteAsync(Quote entity, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task<Quote?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task<IReadOnlyCollection<Quote>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task<IReadOnlyCollection<Quote>> GetAllPagedAsync(int pageNumber, int pageSize,
        CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public IQueryable<Quote> AsQueryable(bool includeErased = false)
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
}







