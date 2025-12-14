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

            // Add InMemory DbContext
            services.AddDbContext<CrmDbContext>(options =>
            {
                options.UseInMemoryDatabase("IntegrationTestsDb");
            });

            // Seed data
            using var scope = services.BuildServiceProvider().CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<CrmDbContext>();
            db.Database.EnsureCreated();
            IntegrationTestDataSeeder.Seed(db);
        });
    }
}