using EBOS.CRM.Contracts.Requests.CRM.Lead;
using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Entities.EBOS;
using EBOS.CRM.Domain.Interfaces.Services;
using EBOS.CRM.Infrastructure.Persistence;
using EBOS.CRM.Infrastructure.Services.CRM;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EBOS.CRM.ApiTests.Infrastructure.Services.CRM;

public class LeadDebtorCheckServiceTest
{
    [Fact]
    public async Task CheckAsync_WhenMatchingDebtorIndividual_ReturnsDebtorInfo()
    {
        var options = BuildOptions();
        await using (var seed = new CrmDbContext(options, new TestCurrentUserContext(0)))
        {
            var status = new Status { Description = "Debtor", CreatedAt = DateTime.UtcNow, CreatedBy = 1 };
            var idType = new IdentificationType { Code = "DNI", Description = "Documento", CreatedAt = DateTime.UtcNow, CreatedBy = 1 };
            seed.Statuses.Add(status);
            seed.IdentificationTypes.Add(idType);
            await seed.SaveChangesAsync();

            var individual = new IndividualCustomer
            {
                TenantId = 1,
                Code = "IND-1",
                Email = "debtor@example.com",
                Phone = "123456789",
                StatusId = status.Id,
                FirstName = "Jane",
                LastName = "Doe",
                BirthDate = new DateTime(1990, 1, 1),
                IdentificationNumber = "12345678A",
                IdentificationTypeId = idType.Id,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = 1
            };
            seed.IndividualCustomers.Add(individual);
            await seed.SaveChangesAsync();
        }

        await using var context = new CrmDbContext(options, new TestCurrentUserContext(0));
        var sut = new LeadDebtorCheckService(context);

        var response = await sut.CheckAsync(new LeadDebtorCheckRequest(1, "debtor@example.com", null, null, "Jane Doe"));

        Assert.True(response.IsDebtor);
        Assert.Equal("Individual", response.CustomerType);
        Assert.Equal("IND-1", response.Code);
    }

    [Fact]
    public async Task CheckAsync_WhenNoDebtorFound_ReturnsNotDebtor()
    {
        var options = BuildOptions();
        await using var context = new CrmDbContext(options, new TestCurrentUserContext(0));
        var sut = new LeadDebtorCheckService(context);

        var response = await sut.CheckAsync(new LeadDebtorCheckRequest(1, "none@example.com", null, null, null));

        Assert.False(response.IsDebtor);
        Assert.Null(response.CustomerId);
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

    private sealed class TestCurrentUserContext(long tenantId) : ICurrentUserContext
    {
        public long UserId => 0;
        public long TenantId => tenantId;
        public string CorrelationId => Guid.NewGuid().ToString("D");
    }
}
