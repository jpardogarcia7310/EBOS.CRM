using EBOS.CRM.Api.IntegrationTests.Infrastructure;
using EBOS.CRM.Application.Services.Interfaces;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace EBOS.CRM.IntegrationTests.Infrastructure;
using EBOS.CRM.Api.Constants;

public sealed class InMemoryAddressWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _inMemoryDbName = $"InMemoryAddressTestsDb-{Guid.NewGuid():N}";
    public InMemoryAddressRepository Repository { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.ConfigureServices(services =>
        {
            var dbDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<CrmDbContext>));
            if (dbDescriptor != null)
            {
                services.Remove(dbDescriptor);
            }

            services.AddDbContext<CrmDbContext>(options =>
            {
                options.UseInMemoryDatabase(_inMemoryDbName);
                options.ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            });

            var repoDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IAddressRepository));
            if (repoDescriptor != null)
            {
                services.Remove(repoDescriptor);
            }

            services.AddSingleton<IAddressRepository>(Repository);

            using var scope = services.BuildServiceProvider().CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<CrmDbContext>();
            db.Database.EnsureCreated();
            var normalizer = scope.ServiceProvider.GetRequiredService<EBOS.CRM.Application.Services.Interfaces.ILookupNormalizationService>();
            normalizer.NormalizeAsync().GetAwaiter().GetResult();
            TestDataSeeder.SeedCountriesAsync(db).GetAwaiter().GetResult();
            TestDataSeeder.SeedAddressTypesAsync(db).GetAwaiter().GetResult();
            TestDataSeeder.SeedIdentificationTypesAsync(db).GetAwaiter().GetResult();
            TestDataSeeder.SeedStatusesAsync(db).GetAwaiter().GetResult();
            normalizer.NormalizeAsync().GetAwaiter().GetResult();

            var currentUserDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(ICurrentUserContext));
            if (currentUserDescriptor != null)
            {
                services.Remove(currentUserDescriptor);
            }

            services.AddScoped<ICurrentUserContext, TestCurrentUserContext>();
        });
    }

    protected override void ConfigureClient(HttpClient client)
    {
        client.DefaultRequestHeaders.Remove(HeaderNames.TenantId);
        client.DefaultRequestHeaders.Add(HeaderNames.TenantId, "1");
        base.ConfigureClient(client);
    }
}


