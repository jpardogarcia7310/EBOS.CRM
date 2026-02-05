using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using EBOS.CRM.Infrastructure.Persistence;
using EBOS.CRM.Application.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EBOS.CRM.Api.IntegrationTests.Infrastructure;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly bool _useTestcontainers;
    private readonly IContainer? _sqlContainer;
    private readonly string? _connectionString;

    private const string SaPassword = "StrongP@ssw0rd2025!";

    public CustomWebApplicationFactory()
    {
        _useTestcontainers = string.Equals(
            Environment.GetEnvironmentVariable("USE_TESTCONTAINERS"),
            "true",
            StringComparison.OrdinalIgnoreCase);

        if (_useTestcontainers)
        {
            _sqlContainer = new ContainerBuilder()
                .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
                .WithEnvironment("ACCEPT_EULA", "Y")
                .WithEnvironment("SA_PASSWORD", SaPassword)
                .WithExposedPort(1433)
                .WithPortBinding(1433, true)
                .WithWaitStrategy(Wait.ForUnixContainer().UntilInternalTcpPortIsAvailable(1433))
                .Build();

            _sqlContainer.StartAsync().GetAwaiter().GetResult();

            var mappedPort = _sqlContainer.GetMappedPublicPort(1433);
            var host = _sqlContainer.Hostname;

            var masterSb = new SqlConnectionStringBuilder
            {
                DataSource = $"{host},{mappedPort}",
                UserID = "sa",
                Password = SaPassword,
                InitialCatalog = "master",
                TrustServerCertificate = true,
                Encrypt = false
            };

            WaitForSqlServerAsync(masterSb.ConnectionString).GetAwaiter().GetResult();

            var sb = new SqlConnectionStringBuilder
            {
                DataSource = $"{host},{mappedPort}",
                UserID = "sa",
                Password = SaPassword,
                InitialCatalog = "TestCrmDb",
                TrustServerCertificate = true,
                Encrypt = false
            };

            _connectionString = sb.ConnectionString;
        }
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        return base.CreateHost(builder);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<CrmDbContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            if (_useTestcontainers && _connectionString != null)
            {
                services.AddDbContext<CrmDbContext>(options =>
                    options.UseSqlServer(_connectionString));
            }
            else
            {
                services.AddDbContext<CrmDbContext>(options =>
                {
                    options.UseInMemoryDatabase("IntegrationTestsDb");
                    options.ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning));
                });
            }

            using var scope = services.BuildServiceProvider().CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<CrmDbContext>();
            db.Database.EnsureCreated();
            TestDataSeeder.SeedCountriesAsync(db).GetAwaiter().GetResult();
            TestDataSeeder.SeedAddressTypesAsync(db).GetAwaiter().GetResult();
            TestDataSeeder.SeedIdentificationTypesAsync(db).GetAwaiter().GetResult();
            TestDataSeeder.SeedStatusesAsync(db).GetAwaiter().GetResult();
        });

        builder.ConfigureTestServices(services =>
        {
            var currentUserDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(ICurrentUserContext));
            if (currentUserDescriptor != null)
            {
                services.Remove(currentUserDescriptor);
            }

            services.AddScoped<ICurrentUserContext, TestCurrentUserContext>();
        });
    }

    public override async ValueTask DisposeAsync()
    {
        if (_sqlContainer != null)
        {
            await _sqlContainer.StopAsync();
            await _sqlContainer.DisposeAsync();
        }

        await base.DisposeAsync();
    }

    private static async Task WaitForSqlServerAsync(string connectionString, int maxRetries = 30, int delayMs = 1000)
    {
        var retries = 0;
        while (true)
        {
            try
            {
                await using var conn = new SqlConnection(connectionString);
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
}


