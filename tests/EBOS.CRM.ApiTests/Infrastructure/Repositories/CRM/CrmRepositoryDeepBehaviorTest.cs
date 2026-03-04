using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Entities.EBOS;
using EBOS.CRM.Domain.Interfaces.Services;
using EBOS.CRM.Infrastructure.Persistence;
using EBOS.CRM.Infrastructure.Repositories.Concrete.CRM;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EBOS.CRM.ApiTests.Infrastructure.Repositories.CRM;

public class CrmRepositoryDeepBehaviorTest
{
    [Fact]
    public async Task AddressRepository_DeleteAsync_SoftDeletes_AndExcludesFromCount()
    {
        var options = BuildOptions();
        await using var context = new CrmDbContext(options, new TestCurrentUserContext(1));
        var (countryId, addressTypeId) = await SeedGeoAsync(context);

        var repo = new AddressRepository(context);
        var address = NewAddress(1, countryId, addressTypeId, "One");
        await repo.AddAsync(address);
        await repo.SaveChangesAsync();

        await repo.DeleteAsync(address);
        await repo.SaveChangesAsync();

        Assert.True(address.Erased);
        Assert.Equal(0, await repo.CountAsync());
        Assert.Single(repo.AsQueryable(includeErased: true));
    }

    [Fact]
    public async Task AccountHierarchyRepository_ReturnsParentsAndPaging_ByTenant()
    {
        var options = BuildOptions();
        await using var context = new CrmDbContext(options, new TestCurrentUserContext(0));
        var status = await SeedStatusAsync(context);
        var a = await AddCorporateAsync(context, 1, status.Id, "A");
        var b = await AddCorporateAsync(context, 1, status.Id, "B");
        var c = await AddCorporateAsync(context, 1, status.Id, "C");
        context.AccountHierarchies.Add(AccountHierarchy.Create(1, a.Id, b.Id, "HOLDING", DateTime.UtcNow.AddDays(-2)));
        context.AccountHierarchies.Add(AccountHierarchy.Create(1, b.Id, c.Id, "HOLDING", DateTime.UtcNow.AddDays(-1)));
        await context.SaveChangesAsync();

        var repo = new AccountHierarchyRepository(context);
        var parents = await repo.GetParentIdsByChildIdsAsync(1, new[] { c.Id });
        var page = await repo.GetByAccountPagedAsync(1, b.Id, 1, 10);
        var count = await repo.CountByAccountAsync(1, b.Id);

        Assert.Contains(b.Id, parents);
        Assert.Equal(2, page.Count);
        Assert.Equal(2, count);
    }

    [Fact]
    public async Task CustomerAddressRepository_PrimaryQueries_Work()
    {
        var options = BuildOptions();
        await using var context = new CrmDbContext(options, new TestCurrentUserContext(0));
        var status = await SeedStatusAsync(context);
        var customer = await AddCustomerAsync(context, 1, status.Id, "CUST-1");
        var (countryId, addressTypeId) = await SeedGeoAsync(context);
        var addressA = NewAddress(1, countryId, addressTypeId, "A");
        var addressB = NewAddress(1, countryId, addressTypeId, "B");
        context.Addresses.AddRange(addressA, addressB);
        await context.SaveChangesAsync();

        context.CustomerAddresses.AddRange(
            new CustomerAddress
            {
                TenantId = 1, CustomerId = customer.Id, AddressId = addressA.Id, IsPrimary = true, IsCurrent = false,
                ValidFrom = DateTime.UtcNow.AddDays(-3), ValidTo = DateTime.UtcNow.AddDays(-2), CreatedAt = DateTime.UtcNow, CreatedBy = 1
            },
            new CustomerAddress
            {
                TenantId = 1, CustomerId = customer.Id, AddressId = addressB.Id, IsPrimary = true, IsCurrent = true,
                ValidFrom = DateTime.UtcNow.AddDays(-1), CreatedAt = DateTime.UtcNow, CreatedBy = 1
            });
        await context.SaveChangesAsync();

        var repo = new CustomerAddressRepository(context);
        Assert.True(await repo.ExistsPrimaryAddressForCustomerAsync(customer.Id));
        var current = await repo.GetCurrentPrimaryAsync(customer.Id);
        Assert.NotNull(current);
        Assert.Equal(addressB.Id, current!.AddressId);
    }

    [Fact]
    public async Task CustomerConsentRepository_LatestPerType_AndCounts_Work()
    {
        var options = BuildOptions();
        await using var context = new CrmDbContext(options, new TestCurrentUserContext(0));
        var status = await SeedStatusAsync(context);
        var customer = await AddCustomerAsync(context, 1, status.Id, "CUST-2");
        var t0 = DateTime.UtcNow.AddHours(-2);
        var t1 = DateTime.UtcNow.AddHours(-1);
        context.CustomerConsents.AddRange(
            CustomerConsent.Create(1, customer.Id, "MARKETING_EMAIL", true, t0, "seed", null),
            CustomerConsent.CreateRevoked(1, customer.Id, "MARKETING_EMAIL", t1, "seed", t1),
            CustomerConsent.Create(1, customer.Id, "PRODUCT_UPDATES_SMS", true, t1, "seed", null));
        await context.SaveChangesAsync();

        var repo = new CustomerConsentRepository(context);
        var latest = await repo.GetLatestByCustomerPagedAsync(1, customer.Id, 1, 10);
        var latestCount = await repo.CountLatestByCustomerAsync(1, customer.Id);
        var totalCount = await repo.CountByCustomerAsync(1, customer.Id);

        Assert.Equal(2, latest.Count);
        Assert.Equal(2, latestCount);
        Assert.Equal(3, totalCount);
        Assert.Contains(latest, x => x.ConsentType == "MARKETING_EMAIL" && x.RevokedAt.HasValue);
    }

    [Fact]
    public async Task CustomerPreferenceRepository_ChannelLookup_AndPaging_Work()
    {
        var options = BuildOptions();
        await using var context = new CrmDbContext(options, new TestCurrentUserContext(0));
        var status = await SeedStatusAsync(context);
        var customer = await AddCustomerAsync(context, 1, status.Id, "CUST-3");
        var channel = new ChannelType
        {
            Descripcion = "EMAIL", IsActive = true, CreatedAt = DateTime.UtcNow, CreatedBy = 1,
            UpdatedAt = DateTime.UtcNow, UpdatedBy = 1
        };
        context.ChannelTypes.Add(channel);
        await context.SaveChangesAsync();
        context.CustomerPreferences.Add(CustomerPreference.Create(1, customer.Id, channel.Id, true, DateTime.UtcNow, 1));
        await context.SaveChangesAsync();

        var repo = new CustomerPreferenceRepository(context);
        var byChannel = await repo.GetByCustomerAndChannelAsync(1, customer.Id, channel.Id);
        var page = await repo.GetByCustomerPagedAsync(1, customer.Id, 1, 10);
        var count = await repo.CountByCustomerAsync(1, customer.Id);

        Assert.NotNull(byChannel);
        Assert.Single(page);
        Assert.Equal(1, count);
    }

    [Fact]
    public async Task CustomerPrivacyRequestRepository_StatusAndActiveFilters_Work()
    {
        var options = BuildOptions();
        await using var context = new CrmDbContext(options, new TestCurrentUserContext(0));
        var status = await SeedStatusAsync(context);
        var customer = await AddCustomerAsync(context, 1, status.Id, "CUST-4");
        var completed = CustomerPrivacyRequest.Create(1, customer.Id, CustomerPrivacyRequest.TypeForget, 1, "r", "c");
        completed.MarkInProgress(1);
        completed.MarkCompleted(1);
        var pending = CustomerPrivacyRequest.Create(1, customer.Id, CustomerPrivacyRequest.TypeForget, 1, "r2", "c2");
        context.CustomerPrivacyRequests.AddRange(completed, pending);
        await context.SaveChangesAsync();

        var repo = new CustomerPrivacyRequestRepository(context);
        var byStatus = await repo.GetByStatusPagedAsync(1, " pending ", 1, 10);
        var active = await repo.GetActiveByCustomerAndTypeAsync(1, customer.Id, " forget ");
        var countPending = await repo.CountByStatusAsync(1, "pending");

        Assert.Single(byStatus);
        Assert.NotNull(active);
        Assert.Equal(CustomerPrivacyRequest.StatusPending, active!.Status);
        Assert.Equal(1, countPending);
    }

    [Fact]
    public async Task CaseActivityRepository_FilteredPaging_AndCounts_Work()
    {
        var options = BuildOptions();
        await using var context = new CrmDbContext(options, new TestCurrentUserContext(0));
        var q = new Queue { TenantId = 1, Name = "Q1", Code = "Q1", IsActive = true, CreatedAt = DateTime.UtcNow, CreatedBy = 1 };
        var s = new Sla { TenantId = 1, Name = "S1", TargetMinutes = 60, IsActive = true, CreatedAt = DateTime.UtcNow, CreatedBy = 1 };
        context.Queues.Add(q);
        context.Slas.Add(s);
        await context.SaveChangesAsync();
        var c = new Case
        {
            TenantId = 1, Title = "Case", Status = Case.StatusOpen, Priority = Case.PriorityHigh, OwnerUserId = 1,
            QueueId = q.Id, SlaId = s.Id, CreatedAt = DateTime.UtcNow.AddDays(-2), CreatedBy = 1
        };
        context.Cases.Add(c);
        await context.SaveChangesAsync();
        context.CaseActivities.AddRange(
            new CaseActivity { TenantId = 1, CaseId = c.Id, Title = "a1", Status = CaseActivity.StatusOpen, CreatedAt = DateTime.UtcNow.AddDays(-2), CreatedBy = 1 },
            new CaseActivity { TenantId = 1, CaseId = c.Id, Title = "a2", Status = CaseActivity.StatusCompleted, CreatedAt = DateTime.UtcNow.AddDays(-1), CreatedBy = 1 });
        await context.SaveChangesAsync();

        var repo = new CaseActivityRepository(context);
        var hasOpen = await repo.HasOpenByCaseIdAsync(c.Id);
        var filtered = await repo.GetAllByCaseIdPagedAsync(c.Id, 1, 10, status: CaseActivity.StatusCompleted);
        var countFiltered = await repo.CountByCaseIdAsync(c.Id, status: CaseActivity.StatusCompleted);

        Assert.True(hasOpen);
        Assert.Single(filtered);
        Assert.Equal(1, countFiltered);
    }

    [Fact]
    public async Task CaseRepository_OpenCountsAndBatch_Work()
    {
        var options = BuildOptions();
        await using var context = new CrmDbContext(options, new TestCurrentUserContext(0));
        var q = new Queue { TenantId = 1, Name = "Q2", Code = "Q2", IsActive = true, CreatedAt = DateTime.UtcNow, CreatedBy = 1 };
        var s = new Sla { TenantId = 1, Name = "S2", TargetMinutes = 60, IsActive = true, CreatedAt = DateTime.UtcNow, CreatedBy = 1 };
        context.Queues.Add(q);
        context.Slas.Add(s);
        await context.SaveChangesAsync();
        context.Cases.AddRange(
            new Case { TenantId = 1, Title = "Open", Status = Case.StatusOpen, Priority = Case.PriorityHigh, OwnerUserId = 1, QueueId = q.Id, SlaId = s.Id, CreatedAt = DateTime.UtcNow, CreatedBy = 1 },
            new Case { TenantId = 1, Title = "Closed", Status = Case.StatusClosed, Priority = Case.PriorityHigh, OwnerUserId = 1, QueueId = q.Id, SlaId = s.Id, CreatedAt = DateTime.UtcNow, CreatedBy = 1 });
        await context.SaveChangesAsync();

        var repo = new CaseRepository(context);
        var batch = await repo.GetOpenSlaBatchAsync(1, 1, 10);
        var countOpen = await repo.CountOpenSlaBatchAsync(1);
        var byQueue = await repo.CountOpenByQueueIdAsync(q.Id);
        var bySla = await repo.CountOpenBySlaIdAsync(s.Id);

        Assert.Single(batch);
        Assert.Equal(1, countOpen);
        Assert.Equal(1, byQueue);
        Assert.Equal(1, bySla);
    }

    [Fact]
    public async Task CustomerMergeHistoryRepository_PagingAndCounts_Work()
    {
        var options = BuildOptions();
        await using var context = new CrmDbContext(options, new TestCurrentUserContext(0));
        context.CustomerMergeHistories.AddRange(
            CustomerMergeHistory.Create(1, 100, 200, "r1", 1, DateTime.UtcNow.AddMinutes(-2)),
            CustomerMergeHistory.Create(1, 100, 201, "r2", 1, DateTime.UtcNow.AddMinutes(-1)),
            CustomerMergeHistory.Create(1, 101, 202, "r3", 1, DateTime.UtcNow));
        await context.SaveChangesAsync();

        var repo = new CustomerMergeHistoryRepository(context);
        var byWinner = await repo.GetByWinnerPagedAsync(1, 100, 1, 10);
        var byMerged = await repo.GetByMergedPagedAsync(1, 201, 1, 10);
        var countWinner = await repo.CountByWinnerAsync(1, 100);
        var countMerged = await repo.CountByMergedAsync(1, 201);

        Assert.Equal(2, byWinner.Count);
        Assert.Single(byMerged);
        Assert.Equal(2, countWinner);
        Assert.Equal(1, countMerged);
    }

    [Fact]
    public async Task CustomerRepository_GetWithAddresses_IncludesAddresses()
    {
        var options = BuildOptions();
        await using var context = new CrmDbContext(options, new TestCurrentUserContext(0));
        var status = await SeedStatusAsync(context);
        var customer = await AddCustomerAsync(context, 1, status.Id, "CUST-5");
        var (countryId, addressTypeId) = await SeedGeoAsync(context);
        var address = NewAddress(1, countryId, addressTypeId, "X");
        context.Addresses.Add(address);
        await context.SaveChangesAsync();
        context.CustomerAddresses.Add(new CustomerAddress
        {
            TenantId = 1,
            CustomerId = customer.Id,
            AddressId = address.Id,
            IsPrimary = true,
            IsCurrent = true,
            ValidFrom = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = 1
        });
        await context.SaveChangesAsync();

        var repo = new CustomerRepository(context);
        var loaded = await repo.GetWithAddressesAsync(customer.Id);

        Assert.NotNull(loaded);
        Assert.Single(loaded!.CustomerAddresses);
        Assert.NotNull(loaded.CustomerAddresses.First().Address);
    }

    private static DbContextOptions<CrmDbContext> BuildOptions()
    {
        var serviceProvider = new ServiceCollection()
            .AddEntityFrameworkInMemoryDatabase()
            .BuildServiceProvider();

        return new DbContextOptionsBuilder<CrmDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
            .UseInternalServiceProvider(serviceProvider)
            .Options;
    }

    private static async Task<Status> SeedStatusAsync(CrmDbContext context)
    {
        var status = new Status { Description = "Active", CreatedAt = DateTime.UtcNow, CreatedBy = 1 };
        context.Statuses.Add(status);
        await context.SaveChangesAsync();
        return status;
    }

    private static async Task<(long countryId, long addressTypeId)> SeedGeoAsync(CrmDbContext context)
    {
        var country = new Country
        {
            Name = "Spain", Iso31661A2Code = "ES", Iso31661A3Code = "ESP", Iso31661NumCode = "724",
            Domain = "es", InternationalPhoneCode = "+34", Currency = "Euro", CurrencyCode = "EUR"
        };
        var addressType = new AddressType { Code = "HOME", Description = "Home", Category = "Main" };
        context.Countries.Add(country);
        context.AddressTypes.Add(addressType);
        await context.SaveChangesAsync();
        return (country.Id, addressType.Id);
    }

    private static async Task<CorporateCustomer> AddCorporateAsync(CrmDbContext context, long tenantId, long statusId, string suffix)
    {
        var entity = new CorporateCustomer
        {
            TenantId = tenantId,
            Code = $"CORP-{suffix}",
            Email = $"corp-{suffix}@example.com",
            Phone = "111111111",
            StatusId = statusId,
            LegalName = $"Corp {suffix}",
            TaxIdentification = $"TAX-{suffix}",
            CreatedAt = DateTime.UtcNow,
            CreatedBy = 1
        };
        context.CorporateCustomers.Add(entity);
        await context.SaveChangesAsync();
        return entity;
    }

    private static async Task<Customer> AddCustomerAsync(CrmDbContext context, long tenantId, long statusId, string code)
    {
        var customer = new Customer
        {
            TenantId = tenantId,
            Code = code,
            Email = $"{code.ToLowerInvariant()}@example.com",
            Phone = "600000000",
            StatusId = statusId,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = 1
        };
        context.Customers.Add(customer);
        await context.SaveChangesAsync();
        return customer;
    }

    private static Address NewAddress(long tenantId, long countryId, long addressTypeId, string suffix)
        => new()
        {
            TenantId = tenantId,
            Street = $"Street {suffix}",
            ExternalNumber = "1",
            City = "City",
            StateOrProvince = "State",
            PostalCode = "28001",
            CountryId = countryId,
            AddressTypeId = addressTypeId,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = 1
        };

    private sealed class TestCurrentUserContext(long tenantId) : ICurrentUserContext
    {
        public long UserId => 1;
        public long TenantId => tenantId;
        public string CorrelationId => Guid.NewGuid().ToString("D");
    }
}
