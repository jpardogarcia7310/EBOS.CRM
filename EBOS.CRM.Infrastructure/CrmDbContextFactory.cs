using Microsoft.EntityFrameworkCore.Design;
using System.Text.Json;

namespace EBOS.CRM.Infrastructure;

public class CrmDbContextFactory : IDesignTimeDbContextFactory<CrmDbContext>
{
    public CrmDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__CrmConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            var basePath = Directory.GetCurrentDirectory();
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                              ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");

            var rootSettings = Path.Combine(basePath, "appsettings.json");
            var apiSettings = Path.Combine(basePath, "EBOS.CRM.Api", "appsettings.json");
            
            var rootSettingsEnv = environment is null
                ? null
                : Path.Combine(basePath, $"appsettings.{environment}.json");
            var apiSettingsEnv = environment is null
                ? null
                : Path.Combine(basePath, "EBOS.CRM.Api", $"appsettings.{environment}.json");

            connectionString = TryReadConnectionString(rootSettings)
                               ?? TryReadConnectionString(rootSettingsEnv)
                               ?? TryReadConnectionString(apiSettings)
                               ?? TryReadConnectionString(apiSettingsEnv);
        }

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=EBOS.CRM;Trusted_Connection=True;TrustServerCertificate=True";
        }

        var optionsBuilder = new DbContextOptionsBuilder<CrmDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new CrmDbContext(optionsBuilder.Options, null, null);
    }

    private static string? TryReadConnectionString(string? path)
    {
        if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
        {
            return null;
        }

        try
        {
            using var stream = File.OpenRead(path);
            using var document = JsonDocument.Parse(stream);
            if (!document.RootElement.TryGetProperty("ConnectionStrings", out var connectionStrings))
            {
                return null;
            }

            return !connectionStrings.TryGetProperty("CrmConnection", out var crmConnection) ? null : crmConnection.GetString();
        }
        catch
        {
            return null;
        }
    }
}
