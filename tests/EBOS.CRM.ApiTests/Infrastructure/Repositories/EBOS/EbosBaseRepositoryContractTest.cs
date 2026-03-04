using EBOS.Core.Primitives.Interfaces;
using EBOS.CRM.Domain.Entities.EBOS;
using EBOS.CRM.Domain.Interfaces.Services;
using EBOS.CRM.Infrastructure.Persistence;
using EBOS.CRM.Infrastructure.Repositories;
using EBOS.CRM.Infrastructure.Repositories.Concrete.EBOS;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EBOS.CRM.ApiTests.Infrastructure.Repositories.EBOS;

public class EbosBaseRepositoryContractTest
{
    [Fact]
    public async Task TenantConfigurationRepository_BaseContract_Works()
    {
        await AssertTenantScopedBaseContractAsync<TenantConfiguration, TenantConfigurationRepository>(
            ctx => new TenantConfigurationRepository(ctx),
            async ctx =>
            {
                ctx.TenantConfigurations.AddRange(
                    new TenantConfiguration { TenantId = 1, Key = "retention_days", ValueJson = "90", UpdatedAt = DateTime.UtcNow, UpdatedBy = 1 },
                    new TenantConfiguration { TenantId = 2, Key = "retention_days", ValueJson = "120", UpdatedAt = DateTime.UtcNow, UpdatedBy = 1 });
                await ctx.SaveChangesAsync();
            });
    }

    [Fact]
    public async Task TenantQuotaRepository_BaseContract_Works()
    {
        await AssertTenantScopedBaseContractAsync<TenantQuota, TenantQuotaRepository>(
            ctx => new TenantQuotaRepository(ctx),
            async ctx =>
            {
                ctx.TenantQuotas.AddRange(
                    new TenantQuota { TenantId = 1, Metric = "api_calls", Limit = 1000, EffectiveFrom = DateTime.UtcNow.AddDays(-1) },
                    new TenantQuota { TenantId = 2, Metric = "api_calls", Limit = 2000, EffectiveFrom = DateTime.UtcNow.AddDays(-1) });
                await ctx.SaveChangesAsync();
            });
    }

    [Fact]
    public async Task TenantUsageMetricRepository_BaseContract_Works()
    {
        await AssertTenantScopedBaseContractAsync<TenantUsageMetric, TenantUsageMetricRepository>(
            ctx => new TenantUsageMetricRepository(ctx),
            async ctx =>
            {
                ctx.TenantUsageMetrics.AddRange(
                    new TenantUsageMetric
                    {
                        TenantId = 1, Metric = "api_calls", Value = 500, PeriodStart = DateTime.UtcNow.AddDays(-1),
                        PeriodEnd = DateTime.UtcNow, Source = "test"
                    },
                    new TenantUsageMetric
                    {
                        TenantId = 2, Metric = "api_calls", Value = 1500, PeriodStart = DateTime.UtcNow.AddDays(-1),
                        PeriodEnd = DateTime.UtcNow, Source = "test"
                    });
                await ctx.SaveChangesAsync();
            });
    }

    private static async Task AssertTenantScopedBaseContractAsync<TEntity, TRepository>(
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

    private sealed class TestCurrentUserContext(long tenantId) : ICurrentUserContext
    {
        public long UserId => 1;
        public long TenantId => tenantId;
        public string CorrelationId => Guid.NewGuid().ToString("D");
    }
}
