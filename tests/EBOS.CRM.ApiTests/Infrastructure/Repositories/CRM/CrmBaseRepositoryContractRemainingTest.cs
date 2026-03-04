using EBOS.Core.Primitives.Interfaces;
using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Entities.EBOS;
using EBOS.CRM.Domain.Interfaces.Services;
using EBOS.CRM.Infrastructure.Persistence;
using EBOS.CRM.Infrastructure.Repositories;
using EBOS.CRM.Infrastructure.Repositories.Concrete.CRM;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EBOS.CRM.ApiTests.Infrastructure.Repositories.CRM;

public class CrmBaseRepositoryContractRemainingTest
{
    [Fact]
    public async Task BankInformationRepository_CommonContract_Works()
    {
        await AssertCommonContractAsync<BankInformation, BankInformationRepository>(
            ctx => new BankInformationRepository(ctx),
            async ctx =>
            {
                var statusId = await EnsureStatusAsync(ctx);
                var c1 = await AddCustomerAsync(ctx, 1, statusId, "BI1");
                var c2 = await AddCustomerAsync(ctx, 2, statusId, "BI2");
                var e1 = new BankInformation { TenantId = 1, CustomerId = c1.Id, Iban = "ES1111111111111111111111", CreatedAt = DateTime.UtcNow, CreatedBy = 1 };
                var e2 = new BankInformation { TenantId = 2, CustomerId = c2.Id, Iban = "ES2222222222222222222222", CreatedAt = DateTime.UtcNow, CreatedBy = 1 };
                ctx.BankInformation.AddRange(e1, e2);
                await ctx.SaveChangesAsync();
            });
    }

    [Fact]
    public async Task BranchOfficeRepository_CommonContract_Works()
    {
        await AssertCommonContractAsync<BranchOffice, BranchOfficeRepository>(
            ctx => new BranchOfficeRepository(ctx),
            async ctx =>
            {
                var statusId = await EnsureStatusAsync(ctx);
                var corp1 = await AddCorporateAsync(ctx, 1, statusId, "BR1");
                var corp2 = await AddCorporateAsync(ctx, 2, statusId, "BR2");
                ctx.BranchOffices.AddRange(
                    new BranchOffice { TenantId = 1, CorporateCustomerId = corp1.Id, Name = "B1", PhoneNumber = "111", CreatedAt = DateTime.UtcNow, CreatedBy = 1 },
                    new BranchOffice { TenantId = 2, CorporateCustomerId = corp2.Id, Name = "B2", PhoneNumber = "222", CreatedAt = DateTime.UtcNow, CreatedBy = 1 });
                await ctx.SaveChangesAsync();
            });
    }

    [Fact]
    public async Task CorporateCustomerRepository_CommonContract_Works()
    {
        await AssertCommonContractAsync<CorporateCustomer, CorporateCustomerRepository>(
            ctx => new CorporateCustomerRepository(ctx),
            async ctx =>
            {
                var statusId = await EnsureStatusAsync(ctx);
                ctx.CorporateCustomers.AddRange(
                    new CorporateCustomer
                    {
                        TenantId = 1, Code = "CC1", Email = "cc1@example.com", Phone = "111", StatusId = statusId,
                        LegalName = "Corp 1", TaxIdentification = "T1", CreatedAt = DateTime.UtcNow, CreatedBy = 1
                    },
                    new CorporateCustomer
                    {
                        TenantId = 2, Code = "CC2", Email = "cc2@example.com", Phone = "222", StatusId = statusId,
                        LegalName = "Corp 2", TaxIdentification = "T2", CreatedAt = DateTime.UtcNow, CreatedBy = 1
                    });
                await ctx.SaveChangesAsync();
            });
    }

    [Fact]
    public async Task CreditAccountRepository_CommonContract_Works()
    {
        await AssertCommonContractAsync<CreditAccount, CreditAccountRepository>(
            ctx => new CreditAccountRepository(ctx),
            async ctx =>
            {
                var statusId = await EnsureStatusAsync(ctx);
                var c1 = await AddCustomerAsync(ctx, 1, statusId, "CA1");
                var c2 = await AddCustomerAsync(ctx, 2, statusId, "CA2");
                ctx.CreditAccounts.AddRange(
                    new CreditAccount { TenantId = 1, CustomerId = c1.Id, MaxAmount = 100, UsedAmount = 10, CreatedAt = DateTime.UtcNow, CreatedBy = 1 },
                    new CreditAccount { TenantId = 2, CustomerId = c2.Id, MaxAmount = 100, UsedAmount = 10, CreatedAt = DateTime.UtcNow, CreatedBy = 1 });
                await ctx.SaveChangesAsync();
            });
    }

    [Fact]
    public async Task CreditTransactionRepository_CommonContract_Works()
    {
        await AssertCommonContractAsync<CreditTransaction, CreditTransactionRepository>(
            ctx => new CreditTransactionRepository(ctx),
            async ctx =>
            {
                var statusId = await EnsureStatusAsync(ctx);
                var c1 = await AddCustomerAsync(ctx, 1, statusId, "CT1");
                var c2 = await AddCustomerAsync(ctx, 2, statusId, "CT2");
                var ca1 = new CreditAccount { TenantId = 1, CustomerId = c1.Id, MaxAmount = 100, UsedAmount = 0, CreatedAt = DateTime.UtcNow, CreatedBy = 1 };
                var ca2 = new CreditAccount { TenantId = 2, CustomerId = c2.Id, MaxAmount = 100, UsedAmount = 0, CreatedAt = DateTime.UtcNow, CreatedBy = 1 };
                ctx.CreditAccounts.AddRange(ca1, ca2);
                await ctx.SaveChangesAsync();
                ctx.CreditTransactions.AddRange(
                    new CreditTransaction
                    {
                        TenantId = 1, CreditAccountId = ca1.Id, Date = DateTime.UtcNow, Amount = 10, Type = "Consumption",
                        ExternalReference = "R1", Comments = "C1", CreatedAt = DateTime.UtcNow, CreatedBy = 1
                    },
                    new CreditTransaction
                    {
                        TenantId = 2, CreditAccountId = ca2.Id, Date = DateTime.UtcNow, Amount = 10, Type = "Consumption",
                        ExternalReference = "R2", Comments = "C2", CreatedAt = DateTime.UtcNow, CreatedBy = 1
                    });
                await ctx.SaveChangesAsync();
            });
    }

    [Fact]
    public async Task IndividualCustomerRepository_CommonContract_Works()
    {
        await AssertCommonContractAsync<IndividualCustomer, IndividualCustomerRepository>(
            ctx => new IndividualCustomerRepository(ctx),
            async ctx =>
            {
                var statusId = await EnsureStatusAsync(ctx);
                var idType = await EnsureIdentificationTypeAsync(ctx);
                ctx.IndividualCustomers.AddRange(
                    new IndividualCustomer
                    {
                        TenantId = 1, Code = "IC1", Email = "ic1@example.com", Phone = "111", StatusId = statusId,
                        FirstName = "A", LastName = "B", BirthDate = new DateTime(1990, 1, 1), IdentificationNumber = "11111111A",
                        IdentificationTypeId = idType, CreatedAt = DateTime.UtcNow, CreatedBy = 1
                    },
                    new IndividualCustomer
                    {
                        TenantId = 2, Code = "IC2", Email = "ic2@example.com", Phone = "222", StatusId = statusId,
                        FirstName = "C", LastName = "D", BirthDate = new DateTime(1990, 1, 1), IdentificationNumber = "22222222B",
                        IdentificationTypeId = idType, CreatedAt = DateTime.UtcNow, CreatedBy = 1
                    });
                await ctx.SaveChangesAsync();
            });
    }

    [Fact]
    public async Task LeadRepository_CommonContract_Works()
    {
        await AssertCommonContractAsync<Lead, LeadRepository>(
            ctx => new LeadRepository(ctx),
            async ctx =>
            {
                ctx.Leads.AddRange(
                    new Lead
                    {
                        TenantId = 1, Source = "Web", Status = "New", OwnerUserId = 1, CompanyName = "A", ContactName = "B",
                        Email = "l1@example.com", Phone = "111", CreatedAt = DateTime.UtcNow, CreatedBy = 1
                    },
                    new Lead
                    {
                        TenantId = 2, Source = "Web", Status = "New", OwnerUserId = 1, CompanyName = "C", ContactName = "D",
                        Email = "l2@example.com", Phone = "222", CreatedAt = DateTime.UtcNow, CreatedBy = 1
                    });
                await ctx.SaveChangesAsync();
            });
    }

    [Fact]
    public async Task QueueRepository_CommonContract_Works()
    {
        await AssertCommonContractAsync<Queue, QueueRepository>(
            ctx => new QueueRepository(ctx),
            async ctx =>
            {
                ctx.Queues.AddRange(
                    new Queue { TenantId = 1, Name = "Q1", Code = "Q1", IsActive = true, CreatedAt = DateTime.UtcNow, CreatedBy = 1 },
                    new Queue { TenantId = 2, Name = "Q2", Code = "Q2", IsActive = true, CreatedAt = DateTime.UtcNow, CreatedBy = 1 });
                await ctx.SaveChangesAsync();
            });
    }

    [Fact]
    public async Task QuoteRepository_CommonContract_Works()
    {
        await AssertCommonContractAsync<Quote, QuoteRepository>(
            ctx => new QuoteRepository(ctx),
            async ctx =>
            {
                var statusId = await EnsureStatusAsync(ctx);
                var stageId = await EnsureOpportunityStageAsync(ctx);
                var c1 = await AddCustomerAsync(ctx, 1, statusId, "Q1C");
                var c2 = await AddCustomerAsync(ctx, 2, statusId, "Q2C");
                var o1 = new Opportunity { TenantId = 1, Name = "O1", StageId = stageId, OwnerUserId = 1, CustomerId = c1.Id, Amount = 100, Probability = 0.5m, CreatedAt = DateTime.UtcNow, CreatedBy = 1 };
                var o2 = new Opportunity { TenantId = 2, Name = "O2", StageId = stageId, OwnerUserId = 1, CustomerId = c2.Id, Amount = 100, Probability = 0.5m, CreatedAt = DateTime.UtcNow, CreatedBy = 1 };
                ctx.Opportunities.AddRange(o1, o2);
                await ctx.SaveChangesAsync();
                ctx.Quotes.AddRange(
                    new Quote { TenantId = 1, OpportunityId = o1.Id, Status = "Draft", SubtotalAmount = 10, DiscountAmount = 0, TotalAmount = 10, CreatedAt = DateTime.UtcNow, CreatedBy = 1 },
                    new Quote { TenantId = 2, OpportunityId = o2.Id, Status = "Draft", SubtotalAmount = 10, DiscountAmount = 0, TotalAmount = 10, CreatedAt = DateTime.UtcNow, CreatedBy = 1 });
                await ctx.SaveChangesAsync();
            });
    }

    [Fact]
    public async Task SlaRepository_CommonContract_Works()
    {
        await AssertCommonContractAsync<Sla, SlaRepository>(
            ctx => new SlaRepository(ctx),
            async ctx =>
            {
                ctx.Slas.AddRange(
                    new Sla { TenantId = 1, Name = "S1", TargetMinutes = 30, IsActive = true, CreatedAt = DateTime.UtcNow, CreatedBy = 1 },
                    new Sla { TenantId = 2, Name = "S2", TargetMinutes = 30, IsActive = true, CreatedAt = DateTime.UtcNow, CreatedBy = 1 });
                await ctx.SaveChangesAsync();
            });
    }

    [Fact]
    public async Task TaxInformationRepository_CommonContract_Works()
    {
        await AssertCommonContractAsync<TaxInformation, TaxInformationRepository>(
            ctx => new TaxInformationRepository(ctx),
            async ctx =>
            {
                var statusId = await EnsureStatusAsync(ctx);
                var c1 = await AddCustomerAsync(ctx, 1, statusId, "T1");
                var c2 = await AddCustomerAsync(ctx, 2, statusId, "T2");
                ctx.TaxInformation.AddRange(
                    new TaxInformation { TenantId = 1, CustomerId = c1.Id, TaxName = "VAT", TaxIdentificationNumber = "A1", CreatedAt = DateTime.UtcNow, CreatedBy = 1 },
                    new TaxInformation { TenantId = 2, CustomerId = c2.Id, TaxName = "VAT", TaxIdentificationNumber = "A2", CreatedAt = DateTime.UtcNow, CreatedBy = 1 });
                await ctx.SaveChangesAsync();
            });
    }

    [Fact]
    public async Task TaxInformationAddressRepository_CommonContract_Works()
    {
        await AssertCommonContractAsync<TaxInformationAddress, TaxInformationAddressRepository>(
            ctx => new TaxInformationAddressRepository(ctx),
            async ctx =>
            {
                var statusId = await EnsureStatusAsync(ctx);
                var c1 = await AddCustomerAsync(ctx, 1, statusId, "TA1");
                var c2 = await AddCustomerAsync(ctx, 2, statusId, "TA2");
                var (countryId, addressTypeId) = await EnsureGeoAsync(ctx);
                var a1 = new Address { TenantId = 1, Street = "S1", ExternalNumber = "1", City = "C", StateOrProvince = "S", PostalCode = "1", CountryId = countryId, AddressTypeId = addressTypeId, CreatedAt = DateTime.UtcNow, CreatedBy = 1 };
                var a2 = new Address { TenantId = 2, Street = "S2", ExternalNumber = "1", City = "C", StateOrProvince = "S", PostalCode = "1", CountryId = countryId, AddressTypeId = addressTypeId, CreatedAt = DateTime.UtcNow, CreatedBy = 1 };
                ctx.Addresses.AddRange(a1, a2);
                await ctx.SaveChangesAsync();
                var t1 = new TaxInformation { TenantId = 1, CustomerId = c1.Id, TaxName = "VAT", TaxIdentificationNumber = "A1", CreatedAt = DateTime.UtcNow, CreatedBy = 1 };
                var t2 = new TaxInformation { TenantId = 2, CustomerId = c2.Id, TaxName = "VAT", TaxIdentificationNumber = "A2", CreatedAt = DateTime.UtcNow, CreatedBy = 1 };
                ctx.TaxInformation.AddRange(t1, t2);
                await ctx.SaveChangesAsync();
                ctx.Set<TaxInformationAddress>().AddRange(
                    new TaxInformationAddress { TenantId = 1, TaxInformationId = t1.Id, AddressId = a1.Id, IsPrimary = true, IsCurrent = true, ValidFrom = DateTime.UtcNow, CreatedAt = DateTime.UtcNow, CreatedBy = 1 },
                    new TaxInformationAddress { TenantId = 2, TaxInformationId = t2.Id, AddressId = a2.Id, IsPrimary = true, IsCurrent = true, ValidFrom = DateTime.UtcNow, CreatedAt = DateTime.UtcNow, CreatedBy = 1 });
                await ctx.SaveChangesAsync();
            });
    }

    private static async Task AssertCommonContractAsync<TEntity, TRepository>(
        Func<CrmDbContext, TRepository> repositoryFactory,
        Func<CrmDbContext, Task> seedAction)
        where TEntity : class, ISoftDeletable
        where TRepository : BaseRepository<TEntity>
    {
        var options = BuildOptions();
        await using (var seedContext = new CrmDbContext(options, new TestCurrentUserContext(0)))
        {
            await seedAction(seedContext);
        }

        await using var context = new CrmDbContext(options, new TestCurrentUserContext(1));
        var repository = repositoryFactory(context);

        var page = await repository.GetAllPagedAsync(1, 10);
        Assert.Single(page);
        Assert.Equal(1, await repository.CountAsync());

        var entity = page.Single();
        await repository.DeleteAsync(entity);
        await repository.SaveChangesAsync();

        Assert.Equal(0, await repository.CountAsync());
        Assert.Single(repository.AsQueryable(includeErased: true));
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

    private static async Task<long> EnsureStatusAsync(CrmDbContext context)
    {
        var status = await context.Statuses.FirstOrDefaultAsync();
        if (status is not null) return status.Id;
        status = new Status { Description = "Active", CreatedAt = DateTime.UtcNow, CreatedBy = 1 };
        context.Statuses.Add(status);
        await context.SaveChangesAsync();
        return status.Id;
    }

    private static async Task<long> EnsureIdentificationTypeAsync(CrmDbContext context)
    {
        var item = await context.IdentificationTypes.FirstOrDefaultAsync();
        if (item is not null) return item.Id;
        item = new IdentificationType { Code = "DNI", Description = "Documento", CreatedAt = DateTime.UtcNow, CreatedBy = 1 };
        context.IdentificationTypes.Add(item);
        await context.SaveChangesAsync();
        return item.Id;
    }

    private static async Task<long> EnsureOpportunityStageAsync(CrmDbContext context)
    {
        var stage = await context.OpportunityStages.FirstOrDefaultAsync();
        if (stage is not null) return stage.Id;
        stage = new OpportunityStage
        {
            TenantId = 0, Name = "Prospecting", Order = 1, DefaultProbability = 0.1m,
            IsClosed = false, IsWon = false
        };
        context.OpportunityStages.Add(stage);
        await context.SaveChangesAsync();
        return stage.Id;
    }

    private static async Task<(long countryId, long addressTypeId)> EnsureGeoAsync(CrmDbContext context)
    {
        var country = await context.Countries.FirstOrDefaultAsync();
        if (country is null)
        {
            country = new Country
            {
                Name = "Spain", Iso31661A2Code = "ES", Iso31661A3Code = "ESP", Iso31661NumCode = "724",
                Domain = "es", InternationalPhoneCode = "+34", Currency = "Euro", CurrencyCode = "EUR"
            };
            context.Countries.Add(country);
        }

        var addressType = await context.AddressTypes.FirstOrDefaultAsync();
        if (addressType is null)
        {
            addressType = new AddressType { Code = "HOME", Description = "Home", Category = "Main" };
            context.AddressTypes.Add(addressType);
        }

        await context.SaveChangesAsync();
        return (country.Id, addressType.Id);
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

    private sealed class TestCurrentUserContext(long tenantId) : ICurrentUserContext
    {
        public long UserId => 1;
        public long TenantId => tenantId;
        public string CorrelationId => Guid.NewGuid().ToString("D");
    }
}
