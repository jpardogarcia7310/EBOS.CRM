using EBOS.CRM.Application.Services.Audit;
using EBOS.CRM.Application.Services.Interfaces;
using EBOS.CRM.Api.Constants;
using EBOS.CRM.Application.Contracts.Requests.Services;
using EBOS.CRM.Application.Contracts.Responses.Services;
using EBOS.CRM.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EBOS.CRM.ApiTests.Fixtures;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["AuditService:Enabled"] = "false"
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

            var auditDescriptors = services.Where(d => d.ServiceType == typeof(IAuditService)).ToList();
            foreach (var descriptor in auditDescriptors)
            {
                services.Remove(descriptor);
            }
            services.AddScoped<IAuditService, NoOpAuditService>();

            // Remove existing DbContext
            var dbOptionsDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<CrmDbContext>));
            if (dbOptionsDescriptor != null)
                services.Remove(dbOptionsDescriptor);
            var contextDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(CrmDbContext));
            if (contextDescriptor != null)
                services.Remove(contextDescriptor);

            // Add InMemory DbContext
            var dbName = $"IntegrationTestsDb_{Guid.NewGuid()}";
            services.AddDbContext<CrmDbContext>(options =>
            {
                options.UseInMemoryDatabase(dbName);
            });

            // Seed data
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

    private sealed class TestCurrentUserContext : ICurrentUserContext
    {
        public long UserId => 1;
        public long TenantId => 1;
        public string CorrelationId => Guid.NewGuid().ToString("D");
    }

    private sealed class NoOpAuditService : IAuditService
    {
        public Task<AuditInsertResponse> InsertAuditAsync(AuditInsertRequest request,
            CancellationToken cancellationToken = default)
            => Task.FromResult(new AuditInsertResponse(true, 0));

        public Task<IReadOnlyCollection<AuditRecord>> GetAllByEntityAsync(string entity,
            CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyCollection<AuditRecord>>(Array.Empty<AuditRecord>());

        public Task<IReadOnlyCollection<AuditRecord>> GetAllByUserIdAsync(long userId,
            CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyCollection<AuditRecord>>(Array.Empty<AuditRecord>());

        public Task<IReadOnlyCollection<AuditRecord>> GetAllByRegisterIdAsync(long registerId,
            CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyCollection<AuditRecord>>(Array.Empty<AuditRecord>());
    }
}


