using EBOS.CRM.Domain.Interfaces.Services;
using EBOS.CRM.Infrastructure;
using EBOS.CRM.Infrastructure.Options;
using EBOS.CRM.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EBOS.CRM.ApiTests.Infrastructure.DependencyInjection;

public class MultiTenantDatabaseStrategyTests
{
    [Fact]
    public void DbContext_UsesTenantConnectionString_FromAppSettings()
    {
        var configuration = BuildConfiguration();
        var services = new ServiceCollection();

        services.AddOptions<MultiTenantOptions>()
            .Bind(configuration.GetSection(MultiTenantOptions.SectionName));
        services.AddScoped<ITenantContext>(_ => new TestTenantContext(7));
        services.AddInfrastructure(configuration);

        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<CrmDbContext>();

        var connectionString = context.Database.GetConnectionString();
        Assert.Equal("Server=base;Database=Tenant_7;", connectionString);
    }

    [Fact]
    public void DbContext_Throws_WhenTemplateMissingTenantToken()
    {
        var configuration = BuildConfiguration(new Dictionary<string, string?>
        {
            [$"{MultiTenantOptions.SectionName}:{nameof(MultiTenantOptions.ConnectionStringTemplate)}"] =
                "Server=base;Database=TenantDb;"
        });
        var services = new ServiceCollection();

        services.AddOptions<MultiTenantOptions>()
            .Bind(configuration.GetSection(MultiTenantOptions.SectionName));
        services.AddScoped<ITenantContext>(_ => new TestTenantContext(7));
        services.AddInfrastructure(configuration);

        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();

        var ex = Assert.Throws<InvalidOperationException>(
            () => scope.ServiceProvider.GetRequiredService<CrmDbContext>());
        Assert.Contains("{tenantId}", ex.Message);
    }

    [Fact]
    public void DbContext_UsesBaseConnection_ForSharedStrategy()
    {
        var configuration = BuildConfiguration(new Dictionary<string, string?>
        {
            [$"{MultiTenantOptions.SectionName}:{nameof(MultiTenantOptions.Strategy)}"] = "Shared"
        });
        var services = new ServiceCollection();

        services.AddOptions<MultiTenantOptions>()
            .Bind(configuration.GetSection(MultiTenantOptions.SectionName));
        services.AddScoped<ITenantContext>(_ => new TestTenantContext(7));
        services.AddInfrastructure(configuration);

        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<CrmDbContext>();

        var connectionString = context.Database.GetConnectionString();
        Assert.Equal("Server=base;Database=BaseDb;", connectionString);
    }

    [Fact]
    public void DbContext_UsesBaseConnection_ForSchemaStrategy()
    {
        var configuration = BuildConfiguration(new Dictionary<string, string?>
        {
            [$"{MultiTenantOptions.SectionName}:{nameof(MultiTenantOptions.Strategy)}"] = "Schema"
        });
        var services = new ServiceCollection();

        services.AddOptions<MultiTenantOptions>()
            .Bind(configuration.GetSection(MultiTenantOptions.SectionName));
        services.AddScoped<ITenantContext>(_ => new TestTenantContext(7));
        services.AddInfrastructure(configuration);

        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<CrmDbContext>();

        var connectionString = context.Database.GetConnectionString();
        Assert.Equal("Server=base;Database=BaseDb;", connectionString);
    }

    [Fact]
    public void DbContext_UsesBaseConnection_When_TenantId_Missing()
    {
        var configuration = BuildConfiguration();
        var services = new ServiceCollection();

        services.AddOptions<MultiTenantOptions>()
            .Bind(configuration.GetSection(MultiTenantOptions.SectionName));
        services.AddScoped<ITenantContext>(_ => new TestTenantContext(0));
        services.AddInfrastructure(configuration);

        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<CrmDbContext>();

        var connectionString = context.Database.GetConnectionString();
        Assert.Equal("Server=base;Database=BaseDb;", connectionString);
    }

    private static IConfiguration BuildConfiguration(IDictionary<string, string?>? overrides = null)
    {
        var basePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "..", "EBOS.CRM.Api");

        var builder = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false)
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:CrmConnection"] = "Server=base;Database=BaseDb;",
                [$"{MultiTenantOptions.SectionName}:{nameof(MultiTenantOptions.Strategy)}"] = "Database",
                [$"{MultiTenantOptions.SectionName}:{nameof(MultiTenantOptions.ConnectionStringTemplate)}"] =
                    "Server=base;Database=Tenant_{tenantId};"
            });

        if (overrides is not null)
        {
            builder.AddInMemoryCollection(overrides);
        }

        return builder.Build();
    }

    private sealed class TestTenantContext(long tenantId) : ITenantContext
    {
        public long TenantId => tenantId;
    }
}
