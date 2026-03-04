using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using EBOS.CRM.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using System.Data;

namespace EBOS.CRM.IntegrationTests.Infrastructure;

[Collection("SqlServerIntegration")]
public sealed class SqlServerMigrationHardeningTests : IAsyncLifetime
{
    private const string SaPassword = "StrongP@ssw0rd2025!";
    private IContainer? _container;
    private string _connectionString = string.Empty;

    [RequiresTestcontainersFact]
    public async Task Migrations_ApplyLatest_AndCreateCustomer360Artifacts()
    {
        await using var db = CreateContext();
        await db.Database.EnsureDeletedAsync();
        await db.Database.MigrateAsync();

        var expectedTables = new (string Schema, string Table)[]
        {
            ("CRM", "CustomerMergeHistories"),
            ("CRM", "CustomerPrivacyRequests"),
            ("EBOS", "AuditOutboxMessages")
        };

        foreach (var (schema, table) in expectedTables)
        {
            var exists = await TableExistsAsync(schema, table);
            exists.Should().BeTrue($"table {schema}.{table} must exist after applying migrations");
        }

        var migrationCount = db.Database.GetMigrations().Count();
        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT COUNT(1) FROM [__EFMigrationsHistory]";
        var applied = Convert.ToInt32(await cmd.ExecuteScalarAsync());
        applied.Should().Be(migrationCount);
    }

    [RequiresTestcontainersFact]
    public async Task Migrations_CanRollbackOneStep_AndReapplyLatest()
    {
        await using var db = CreateContext();
        await db.Database.EnsureDeletedAsync();
        var migrator = db.Database.GetService<IMigrator>();
        var migrations = db.Database.GetMigrations().ToArray();
        migrations.Length.Should().BeGreaterThan(1);

        var latest = migrations[^1];
        var previous = migrations[^2];

        await migrator.MigrateAsync(latest);
        await migrator.MigrateAsync(previous);

        var lastAppliedAfterRollback = (await db.Database.GetAppliedMigrationsAsync()).LastOrDefault();
        lastAppliedAfterRollback.Should().Be(previous);

        await migrator.MigrateAsync(latest);
        var lastAppliedAfterReapply = (await db.Database.GetAppliedMigrationsAsync()).LastOrDefault();
        lastAppliedAfterReapply.Should().Be(latest);
    }

    [RequiresTestcontainersFact]
    public async Task SqlServer_WriteContention_OnSameResource_ProducesTimeoutThenRecovers()
    {
        await using var db = CreateContext();
        await db.Database.EnsureDeletedAsync();
        await db.Database.MigrateAsync();

        var statusId = await EnsureStatusAsync("WriteContention-Active");
        var customerId = await InsertCustomerAsync(statusId, tenantId: 1, codePrefix: "WC");

        await using var holderConnection = new SqlConnection(_connectionString);
        await holderConnection.OpenAsync();
        await using var holderTx = (SqlTransaction)await holderConnection.BeginTransactionAsync(IsolationLevel.Serializable);

        await using (var holderCmd = holderConnection.CreateCommand())
        {
            holderCmd.Transaction = holderTx;
            holderCmd.CommandText = """
                                    UPDATE [CRM].[Customers]
                                    SET [Phone] = [Phone]
                                    WHERE [Id] = @id
                                    """;
            holderCmd.Parameters.AddWithValue("@id", customerId);
            await holderCmd.ExecuteNonQueryAsync();
        }

        var blockedTask = Task.Run(async () =>
        {
            await using var blockedConnection = new SqlConnection(_connectionString);
            await blockedConnection.OpenAsync();
            await using var blockedCmd = blockedConnection.CreateCommand();
            blockedCmd.CommandTimeout = 2;
            blockedCmd.CommandText = """
                                     UPDATE [CRM].[Customers]
                                     SET [Phone] = @phone
                                     WHERE [Id] = @id
                                     """;
            blockedCmd.Parameters.AddWithValue("@id", customerId);
            blockedCmd.Parameters.AddWithValue("@phone", "34699999999");
            return await blockedCmd.ExecuteNonQueryAsync();
        });

        await Task.Delay(500);
        await Assert.ThrowsAnyAsync<SqlException>(() => blockedTask);
        await holderTx.RollbackAsync();

        await using var recoveryConnection = new SqlConnection(_connectionString);
        await recoveryConnection.OpenAsync();
        await using (var recoveryCmd = recoveryConnection.CreateCommand())
        {
            recoveryCmd.CommandText = """
                                      UPDATE [CRM].[Customers]
                                      SET [Phone] = @phone
                                      WHERE [Id] = @id
                                      """;
            recoveryCmd.Parameters.AddWithValue("@id", customerId);
            recoveryCmd.Parameters.AddWithValue("@phone", "34688888888");
            var affected = await recoveryCmd.ExecuteNonQueryAsync();
            affected.Should().Be(1);
        }
    }

    [RequiresTestcontainersFact]
    public async Task SqlServer_EfExecutionStrategy_RetriesOnTransientDeadlockError()
    {
        await using var db = CreateContextWithRetry();
        await db.Database.EnsureDeletedAsync();
        await db.Database.MigrateAsync();

        var strategy = db.Database.CreateExecutionStrategy();
        strategy.RetriesOnFailure.Should().BeTrue();

        var attempts = 0;
        var result = await strategy.ExecuteAsync(async () =>
        {
            attempts++;
            await using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            await using var cmd = conn.CreateCommand();

            if (attempts == 1)
            {
                throw new TimeoutException("Simulated transient timeout to validate SQL Server execution strategy retry.");
            }

            cmd.CommandText = "SELECT 1;";
            return Convert.ToInt32(await cmd.ExecuteScalarAsync());
        });

        attempts.Should().BeGreaterThan(1);
        result.Should().Be(1);
    }

    [RequiresTestcontainersFact]
    public async Task SqlServer_TransactionRollback_PreservesConsistencyAfterFailure()
    {
        await using var db = CreateContext();
        await db.Database.EnsureDeletedAsync();
        await db.Database.MigrateAsync();

        var statusId = await EnsureStatusAsync("Rollback-Active");
        var marker = $"RB-{Guid.NewGuid():N}"[..12];

        await using (var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable))
        {
            db.Database.SetCommandTimeout(30);
            db.CorporateCustomers.Add(new Domain.Entities.CRM.CorporateCustomer
            {
                TenantId = 1,
                Code = marker,
                Email = $"{marker}@example.com",
                Phone = "34677777777",
                StatusId = statusId,
                LegalName = $"Corp {marker}",
                TaxIdentification = $"B{Random.Shared.Next(10000000, 99999999)}",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = 1
            });

            await db.SaveChangesAsync();
            await tx.RollbackAsync();
        }

        await using var verify = CreateContext();
        await verify.Database.MigrateAsync();
        var exists = await verify.Customers.AsNoTracking().AnyAsync(x => x.Code == marker);
        exists.Should().BeFalse("rolled-back transaction must not leave persisted customer rows");
    }

    public async Task InitializeAsync()
    {
        if (!UseTestcontainersEnabled())
        {
            return;
        }

        _container = new ContainerBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .WithEnvironment("ACCEPT_EULA", "Y")
            .WithEnvironment("SA_PASSWORD", SaPassword)
            .WithExposedPort(1433)
            .WithPortBinding(1433, true)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilInternalTcpPortIsAvailable(1433))
            .Build();

        await _container.StartAsync();
        var mappedPort = _container.GetMappedPublicPort(1433);
        var host = _container.Hostname;

        var sb = new SqlConnectionStringBuilder
        {
            DataSource = $"{host},{mappedPort}",
            UserID = "sa",
            Password = SaPassword,
            InitialCatalog = "master",
            TrustServerCertificate = true,
            Encrypt = false
        };
        await WaitForSqlServerAsync(sb.ConnectionString);

        sb.InitialCatalog = $"CrmMigrationHardening_{Guid.NewGuid():N}";
        _connectionString = sb.ConnectionString;
    }

    public async Task DisposeAsync()
    {
        if (_container is null)
        {
            return;
        }

        await _container.StopAsync();
        await _container.DisposeAsync();
    }

    private CrmDbContext CreateContext()
    {
        if (!UseTestcontainersEnabled())
        {
            throw new InvalidOperationException("Set USE_TESTCONTAINERS=true to run SQL Server migration hardening tests.");
        }

        var options = new DbContextOptionsBuilder<CrmDbContext>()
            .UseSqlServer(_connectionString)
            .Options;
        return new CrmDbContext(options);
    }

    private async Task<bool> TableExistsAsync(string schema, string tableName)
    {
        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();
        await using var cmd = conn.CreateCommand();
        cmd.CommandText =
            "SELECT COUNT(1) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = @schema AND TABLE_NAME = @name";
        cmd.Parameters.AddWithValue("@schema", schema);
        cmd.Parameters.AddWithValue("@name", tableName);
        var count = Convert.ToInt32(await cmd.ExecuteScalarAsync());
        return count > 0;
    }

    private static async Task WaitForSqlServerAsync(string connectionString, int retries = 45, int delayMs = 1000)
    {
        for (var attempt = 1; attempt <= retries; attempt++)
        {
            try
            {
                await using var conn = new SqlConnection(connectionString);
                await conn.OpenAsync();
                return;
            }
            catch when (attempt < retries)
            {
                await Task.Delay(delayMs);
            }
        }
    }

    private static bool UseTestcontainersEnabled() =>
        string.Equals(Environment.GetEnvironmentVariable("USE_TESTCONTAINERS"), "true",
            StringComparison.OrdinalIgnoreCase);

    private CrmDbContext CreateContextWithRetry()
    {
        if (!UseTestcontainersEnabled())
        {
            throw new InvalidOperationException("Set USE_TESTCONTAINERS=true to run SQL Server migration hardening tests.");
        }

        var options = new DbContextOptionsBuilder<CrmDbContext>()
            .UseSqlServer(_connectionString, sql =>
            {
                sql.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromMilliseconds(200), errorNumbersToAdd: null);
            })
            .Options;
        return new CrmDbContext(options);
    }

    private async Task<long> EnsureStatusAsync(string description)
    {
        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = """
                          INSERT INTO [EBOS].[Statuses] ([Description], [CreatedAt], [CreatedBy])
                          VALUES (@desc, SYSUTCDATETIME(), 1);
                          SELECT CAST(SCOPE_IDENTITY() AS BIGINT);
                          """;
        cmd.Parameters.AddWithValue("@desc", description);
        return Convert.ToInt64(await cmd.ExecuteScalarAsync());
    }

    private async Task<long> InsertCustomerAsync(long statusId, long tenantId, string codePrefix)
    {
        var code = $"{codePrefix}-{Guid.NewGuid():N}"[..12];
        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = """
                          INSERT INTO [CRM].[Customers]
                          ([TenantId], [Code], [Email], [Phone], [CreatedAt], [CreatedBy], [StatusId], [Erased], [CustomerType], [LegalName], [TaxIdentification])
                          VALUES (@tenantId, @code, @email, @phone, SYSUTCDATETIME(), 1, @statusId, 0, 'Corporate', @legalName, @taxId);
                          SELECT CAST(SCOPE_IDENTITY() AS BIGINT);
                          """;
        cmd.Parameters.AddWithValue("@tenantId", tenantId);
        cmd.Parameters.AddWithValue("@code", code);
        cmd.Parameters.AddWithValue("@email", $"{code}@example.com");
        cmd.Parameters.AddWithValue("@phone", "34655555555");
        cmd.Parameters.AddWithValue("@legalName", $"Corp {code}");
        cmd.Parameters.AddWithValue("@taxId", $"B{Random.Shared.Next(10000000, 99999999)}");
        cmd.Parameters.AddWithValue("@statusId", statusId);
        return Convert.ToInt64(await cmd.ExecuteScalarAsync());
    }
}
