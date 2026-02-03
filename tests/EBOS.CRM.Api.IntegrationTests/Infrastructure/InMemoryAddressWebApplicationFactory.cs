using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace EBOS.CRM.Api.IntegrationTests.Infrastructure;

public sealed class InMemoryAddressWebApplicationFactory : WebApplicationFactory<Program>
{
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
                options.UseInMemoryDatabase("InMemoryAddressTestsDb");
                options.ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            });

            var repoDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IAddressRepository));
            if (repoDescriptor != null)
            {
                services.Remove(repoDescriptor);
            }

            services.AddSingleton<IAddressRepository>(Repository);
        });
    }
}


