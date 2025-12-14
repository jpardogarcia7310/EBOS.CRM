using EBOS.CRM.Domain.Interfaces.Repositories;
using EBOS.CRM.Infrastructure.Persistence;
using EBOS.CRM.Infrastructure.Repositories.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EBOS.CRM.Infrastructure;

public static partial class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // DbContext registration
        services.AddDbContext<CrmDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("CrmDb")));

        // Repositories base (AddScoped for per-request lifetime)
        services.AddScoped<ICountryRepository, CountryRepository>();

        // Register Handlers or Infrastructure-specific services (if any, e.g. messaging services, file storage, etc.)
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(AssemblyReference).Assembly));

        return services;
    }
}
