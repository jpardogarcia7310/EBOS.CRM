using EBOS.CRM.Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EBOS.CRM.Api.IntegrationTests.Infrastructure;

public static class DatabaseUtilities
{
    /// <summary>
    /// Limpia la tabla Countries y resetea el identity.
    /// Se ejecuta sin transacciones explícitas para evitar conflictos con SqlServerRetryingExecutionStrategy.
    /// </summary>
    public static async Task ResetCountriesAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CrmDbContext>();

        // Ejecutamos directamente los comandos de limpieza
        await db.Database.ExecuteSqlRawAsync("DELETE FROM dbo.Countries;");
        await db.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT('dbo.Countries', RESEED, 0);");
    }

    /// <summary>
    /// Limpia todas las tablas de la base de datos de pruebas.
    /// Útil para escenarios de integración donde se necesita un estado inicial limpio.
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
            -- Aquí puedes añadir más tablas si lo necesitas
        ";

        await cmd.ExecuteNonQueryAsync();
    }

    /// <summary>
    /// Aplica migraciones o EnsureCreated para garantizar que el esquema está listo.
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