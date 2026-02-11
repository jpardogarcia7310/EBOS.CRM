using EBOS.CRM.Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EBOS.CRM.IntegrationTests.Infrastructure;

public static class DatabaseUtilities
{
    /// <summary>
    /// Clear the Countries table and reset the identity.
    /// It runs without explicit transactions to avoid conflicts with SqlServerRetryingExecutionStrategy.
    /// </summary>
    public static async Task ResetCountriesAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CrmDbContext>();

        // We directly execute the cleaning commands
        await db.Database.ExecuteSqlRawAsync("DELETE FROM dbo.Countries;");
        await db.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT('dbo.Countries', RESEED, 0);");
    }

    /// <summary>
    /// Clean all tables in the test database.
    /// Useful for integration scenarios where a clean initial state is needed.
    /// </summary>
    public static async Task ResetAllAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CrmDbContext>();

        var conn = (SqlConnection)db.Database.GetDbConnection();
        await conn.OpenAsync();

        var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            DELETE FROM dbo.Countries;
            DBCC CHECKIDENT('dbo.Countries', RESEED, 0);
            -- You can add more tables here if needed
        ";

        await cmd.ExecuteNonQueryAsync();
    }

    /// <summary>
    /// Apply migrations or EnsureCreated to guarantee that the schema is ready.
    /// </summary>
    public static void EnsureDatabaseReady(IServiceProvider services)
    {
        using var scope = services.CreateScope();
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
}


