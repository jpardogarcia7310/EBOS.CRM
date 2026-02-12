using EBOS.CRM.Domain.Interfaces.Services;
using EBOS.CRM.Infrastructure.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace EBOS.CRM.IntegrationTests.Infrastructure;

public static class SqliteCrmContextFactory
{
    public static CrmDbContext Create(long tenantId = 1)
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        connection.CreateFunction<string?, int?>("LEN", value => value?.Length);
        connection.CreateFunction("SYSUTCDATETIME", () => DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fffffff"));

        var options = new DbContextOptionsBuilder<CrmDbContext>()
            .UseSqlite(connection)
            .Options;

        var context = new CrmDbContext(options, new TestTenantContext(tenantId));
        context.Database.EnsureCreated();
        return context;
    }

    private sealed class TestTenantContext(long tenantId) : ITenantContext
    {
        public long TenantId => tenantId;
    }
}
