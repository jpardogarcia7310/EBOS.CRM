using EBOS.CRM.Api.Constants;
using EBOS.CRM.ApiTests.Fixtures;
using EBOS.CRM.Domain.Interfaces.Services;
using EBOS.CRM.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EBOS.CRM.ConcurrencyTests.Fixtures;

public sealed class ConcurrencyWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram>
    where TProgram : class
{
    private readonly string _connectionString = $"DataSource=ConcurrencyTests_{Guid.NewGuid():N};" +
                                                $"Mode=Memory;Cache=Shared";
    private readonly SqliteConnection _masterConnection;

    public ConcurrencyWebApplicationFactory()
    {
        _masterConnection = new SqliteConnection(_connectionString);
    }

    protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
    {
        _masterConnection.Open();
        ConfigureConnection(_masterConnection);

        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["AuditService:Enabled"] = "false",
                ["Authentication:Enabled"] = "false"
            });
        });

        builder.ConfigureServices(services =>
        {
            var currentUserDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(ICurrentUserContext));
            if (currentUserDescriptor != null)
            {
                services.Remove(currentUserDescriptor);
            }
            services.AddScoped<ICurrentUserContext>(_ => new TestCurrentUserContext());

            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<CrmDbContext>));
            if (descriptor != null)
                services.Remove(descriptor);
            var contextDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(CrmDbContext));
            if (contextDescriptor != null)
                services.Remove(contextDescriptor);

            services.AddDbContext<CrmDbContext>(options =>
            {
                var connection = new SqliteConnection(_connectionString);
                connection.Open();
                ConfigureConnection(connection);
                options.UseSqlite(connection);
            });

            using var scope = services.BuildServiceProvider().CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<CrmDbContext>();
            db.Database.EnsureCreated();
            IntegrationTestCrmDataSeeder.Seed(db);
        });
    }

    protected override void ConfigureClient(HttpClient client)
    {
        client.DefaultRequestHeaders.Remove(HeaderNames.TenantId);
        client.DefaultRequestHeaders.Add(HeaderNames.TenantId, "1");
        base.ConfigureClient(client);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            _masterConnection.Dispose();
        }
    }

    private static void ConfigureConnection(SqliteConnection connection)
    {
        connection.CreateFunction<string?, int>("LEN", value => value?.Length ?? 0);
        connection.CreateFunction("SYSUTCDATETIME", () => DateTime.UtcNow);
        connection.CreateFunction("GETUTCDATE", () => DateTime.UtcNow);
        using var pragmaCommand = connection.CreateCommand();
        pragmaCommand.CommandText = string.Join(' ', new[]
        {
            "PRAGMA ignore_check_constraints = ON;",
            "PRAGMA journal_mode = WAL;",
            "PRAGMA synchronous = NORMAL;",
            "PRAGMA busy_timeout = 5000;"
        });
        pragmaCommand.ExecuteNonQuery();
    }

    private sealed class TestCurrentUserContext : ICurrentUserContext
    {
        public long UserId => 1;
        public long TenantId => 1;
        public string CorrelationId => Guid.NewGuid().ToString("D");
    }
}
