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

namespace EBOS.CRM.Api.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncDisposable
{
    private readonly IContainer _sqlContainer;
    private readonly string _connectionString;
    private ILogger<CustomWebApplicationFactory>? _logger;
    private bool _isStarted;

    // Centraliza la contraseña para evitar desincronizaciones
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

        // Arrancar el contenedor y marcar estado
        _sqlContainer.StartAsync().GetAwaiter().GetResult();
        _isStarted = true;

        // Obtener host/puerto mapeado
        var mappedPort = _sqlContainer.GetMappedPublicPort(1433);
        var host = _sqlContainer.Hostname;

        // 1) Esperar a que el servidor acepte conexiones usando la base 'master'
        var masterSb = new SqlConnectionStringBuilder
        {
            DataSource = $"{host},{mappedPort}",
            UserID = "sa",
            Password = saPassword,
            InitialCatalog = "master",
            TrustServerCertificate = true,
            Encrypt = false
        };

        // Espera robusta hasta que SQL Server acepte conexiones en 'master'
        WaitForSqlServerAsync(masterSb.ConnectionString).GetAwaiter().GetResult();

        // 2) Construir la connection string que usará el DbContext (apunta a TestCrmDb)
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
    }

    // Exponer la connection string para depuración si lo necesitas
    public string ConnectionString => _connectionString;

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        return base.CreateHost(builder);
    }

    /// <summary>
    /// Forzar creación del host y aplicar migraciones/EnsureCreated.
    /// Útil para llamar desde tests antes de seedear.
    /// </summary>
    public void EnsureDatabaseCreated()
    {
        // Forzar creación del host
        _ = this.CreateClient();

        using var scope = this.Services.CreateScope();
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
        // Aquí usamos correctamente el lambda con 'services' en su scope
        builder.ConfigureServices(services =>
        {
            // Intentar resolver un logger si existe
            try
            {
                var tempProvider = services.BuildServiceProvider();
                _logger = tempProvider.GetService<ILogger<CustomWebApplicationFactory>>();
            }
            catch
            {
                // no crítico
            }

            // --- CREAR LA BASE TestCrmDb SI NO EXISTE (conectando a master) ---
            // Construir cadena para master (usar DataSource del _connectionString)
            var masterSb = new SqlConnectionStringBuilder
            {
                DataSource = new SqlConnectionStringBuilder(_connectionString).DataSource,
                UserID = "sa",
                Password = saPassword,
                InitialCatalog = "master",
                TrustServerCertificate = true,
                Encrypt = false
            };

            try
            {
                using var conn = new SqlConnection(masterSb.ConnectionString);
                conn.Open();
                using var cmd = conn.CreateCommand();
                cmd.CommandText = @"
                    IF DB_ID(N'TestCrmDb') IS NULL
                    BEGIN
                        CREATE DATABASE [TestCrmDb];
                    END";
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error creating TestCrmDb database.");
                throw;
            }

            // --- REEMPLAZAR/REGISTRAR DbContext apuntando a TestCrmDb ---
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<CrmDbContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<CrmDbContext>(options =>
                options.UseSqlServer(_connectionString, sql => sql.EnableRetryOnFailure()));

            // Construir provider y aplicar migraciones/EnsureCreated
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<CrmDbContext>();

            // Preferible: aplicar migraciones si las tienes; si no, EnsureCreated()
            try
            {
                db.Database.Migrate();
            }
            catch (Exception migrateEx)
            {
                _logger?.LogWarning(migrateEx, "Migrate failed, trying EnsureCreated.");
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