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
}
