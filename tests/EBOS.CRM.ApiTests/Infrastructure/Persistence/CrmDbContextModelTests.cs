using EBOS.CRM.Domain.Entities;
using EBOS.CRM.Domain.Entities.EBOS;
using EBOS.CRM.Domain.Interfaces.Services;
using EBOS.CRM.Domain.Interfaces.Services.EBOS;
using EBOS.CRM.Infrastructure.Options;
using EBOS.CRM.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OptionsProvider = Microsoft.Extensions.Options.Options;

namespace EBOS.CRM.ApiTests.Infrastructure.Persistence;

public class CrmDbContextModelTests
{
    [Fact]
    public void Model_ConfiguresTenantQueryFilters_ForTenantEntities()
    {
        var options = BuildOptions();
        using var context = new CrmDbContext(options, new TestTenantContext(1));

        var tenantEntities = context.Model.GetEntityTypes()
            .Where(e => e.FindProperty("TenantId") != null)
            .Where(e => e.BaseType == null)
            .ToList();

        Assert.NotEmpty(tenantEntities);
        foreach (var entity in tenantEntities)
        {
            Assert.NotNull(entity.GetQueryFilter());
        }
    }

    [Fact]
    public void Model_UsesCurrentTenantId_InTenantQueryFilters()
    {
        var options = BuildOptions();
        using var context = new CrmDbContext(options, new TestTenantContext(1));

        var tenantEntities = context.Model.GetEntityTypes()
            .Where(e => e.FindProperty("TenantId") != null)
            .Where(e => e.BaseType == null)
            .ToList();

        foreach (var filter in tenantEntities.Select(entity => entity.GetQueryFilter()))
        {
            Assert.NotNull(filter);
            var filterText = filter.ToString();
            Assert.Contains("CurrentTenantId", filterText);
        }
    }

    [Fact]
    public void Model_OverridesSchema_ForSchemaStrategy()
    {
        var options = BuildOptions();
        var multiTenantOptions = OptionsProvider.Create(new MultiTenantOptions
        {
            Strategy = MultiTenantStrategy.Schema,
            SchemaPrefix = "Tenant_",
            SchemaTargets = ["CRM"]
        });

        using var context = new CrmDbContext(options, new TestTenantContext(9), multiTenantOptions);

        var tenantSchema = "Tenant_9";
        var tenantEntities = context.Model.GetEntityTypes()
            .Where(e => string.Equals(e.GetSchema(), tenantSchema, StringComparison.OrdinalIgnoreCase))
            .ToList();

        Assert.NotEmpty(tenantEntities);
    }

    [Fact]
    public void Model_OnlyOverrides_Targeted_Schemas()
    {
        var options = BuildOptions();
        var multiTenantOptions = OptionsProvider.Create(new MultiTenantOptions
        {
            Strategy = MultiTenantStrategy.Schema,
            SchemaPrefix = "Tenant_",
            SchemaTargets = ["CRM"]
        });

        using var context = new CrmDbContext(options, new TestTenantContext(9), multiTenantOptions);

        var customerSchema = context.Model.FindEntityType(typeof(EBOS.CRM.Domain.Entities.CRM.Customer))?.GetSchema();
        var countrySchema = context.Model.FindEntityType(typeof(Country))?.GetSchema();
        var roleSchema = context.Model.FindEntityType(typeof(EBOS.CRM.Domain.Entities.Identity.Role))?.GetSchema();

        Assert.Equal("Tenant_9", customerSchema);
        Assert.Equal("EBOS", countrySchema);
        Assert.Equal("IAM", roleSchema);
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

    private sealed class TestTenantContext(long tenantId) : ITenantContext
    {
        public long TenantId => tenantId;
    }
}
