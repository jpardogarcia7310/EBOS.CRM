using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using EBOS.CRM.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EBOS.CRM.Api.IntegrationTests.Infrastructure;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncDisposable
{
    private readonly IContainer _sqlContainer;
    private readonly string _connectionString;
    private readonly ILogger<CustomWebApplicationFactory> _logger;
    private bool _isStarted;

    private const string saPassword = "StrongP@ssw0rd2025!";

    public CustomWebApplicationFactory()
    {
        // Construir el contenedor SQL Server
        _sqlContainer = new ContainerBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .WithEnvironment("ACCEPT_EULA", "Y")
            .WithEnvironment("SA_PASSWORD", saPassword)
            .WithExposedPort(1433)
            .WithPortBinding(1433, true)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilInternalTcpPortIsAvailable(1433))
            .Build();

        _sqlContainer.StartAsync().GetAwaiter().GetResult();
        _isStarted = true;

        var mappedPort = _sqlContainer.GetMappedPublicPort(1433);
        var host = _sqlContainer.Hostname;

        // Cadena de conexión a master para esperar disponibilidad
        var masterSb = new SqlConnectionStringBuilder
        {
            DataSource = $"{host},{mappedPort}",
            UserID = "sa",
            Password = saPassword,
            InitialCatalog = "master",
            TrustServerCertificate = true,
            Encrypt = false
        };

        WaitForSqlServerAsync(masterSb.ConnectionString).GetAwaiter().GetResult();

        // Cadena de conexión a la base de pruebas
        var sb = new SqlConnectionStringBuilder
        {
            DataSource = $"{host},{mappedPort}",
            UserID = "sa",
            Password = saPassword,
            InitialCatalog = "TestCrmDb",
            TrustServerCertificate = true,
            Encrypt = false
        };

        _connectionString = sb.ConnectionString;

        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
        });

        _logger = loggerFactory.CreateLogger<CustomWebApplicationFactory>();
    }

    public string ConnectionString => _connectionString;

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        return base.CreateHost(builder);
    }

    public void EnsureDatabaseCreated()
    {
        _ = CreateClient();

        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CrmDbContext>();

        try
        {
            db.Database.Migrate();
        }
        catch
        {
            db.Database.EnsureCreated();
        }
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Elimina el DbContext original
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<CrmDbContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Registra el DbContext apuntando a la base de pruebas
            services.AddDbContext<CrmDbContext>(options =>
                options.UseSqlServer(_connectionString));

            // Aplica migraciones o EnsureCreated
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<CrmDbContext>();
            try
            {
                db.Database.Migrate();
            }
            catch
            {
                db.Database.EnsureCreated();
            }
        });
    }

    private static async Task WaitForSqlServerAsync(string connectionString, int maxRetries = 30, int delayMs = 1000)
    {
        var retries = 0;
        while (true)
        {
            try
            {
                using var conn = new SqlConnection(connectionString);
                await conn.OpenAsync();
                return;
            }
            catch
            {
                retries++;
                if (retries >= maxRetries) throw;
                await Task.Delay(delayMs);
            }
        }
    }

    public override async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);

        try
        {
            if (_isStarted)
            {
                await _sqlContainer.StopAsync();
                await _sqlContainer.DisposeAsync();
                _isStarted = false;
            }
        }
        catch (Exception ex)
        {
            _logger?.LogWarning(ex, "Error stopping testcontainer during DisposeAsync.");
        }

        try
        {
            await base.DisposeAsync();
        }
        catch (Exception ex)
        {
            _logger?.LogWarning(ex, "Error disposing base WebApplicationFactory.");
        }
    }
}