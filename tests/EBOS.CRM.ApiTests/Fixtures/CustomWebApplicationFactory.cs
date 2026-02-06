using EBOS.CRM.Application.Services.Interfaces;
using EBOS.CRM.Api.Constants;
using EBOS.CRM.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EBOS.CRM.ApiTests.Fixtures;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var currentUserDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(ICurrentUserContext));
            if (currentUserDescriptor != null)
            {
                services.Remove(currentUserDescriptor);
            }
            services.AddScoped<ICurrentUserContext>(_ => new TestCurrentUserContext());

            // Remove existing DbContext
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<CrmDbContext>));
            if (descriptor != null)
                services.Remove(descriptor);
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
        public long UserId => 0;
        public long TenantId => 1;
        public string CorrelationId => Guid.NewGuid().ToString("D");
    }
}


