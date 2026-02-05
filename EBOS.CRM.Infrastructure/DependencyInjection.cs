using EBOS.CRM.Application.Services.Interfaces;
using EBOS.CRM.Domain.Interfaces.Repositories;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Infrastructure.Repositories.Concrete;
using EBOS.CRM.Infrastructure.Repositories.Concrete.CRM;
using EBOS.CRM.Infrastructure.Services.Audit;
using EBOS.CRM.Infrastructure.Services.Security;
using Microsoft.Extensions.Configuration;

namespace EBOS.CRM.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMemoryCache();

        // DbContext registration
        services.AddDbContext<CrmDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("CrmConnection")));

        services.AddOptions<AuditServiceOptions>()
            .Bind(configuration.GetSection(AuditServiceOptions.SectionName));
        services.AddHttpClient<IAuditService, AuditServiceClient>(client =>
        {
            var options = configuration.GetSection(AuditServiceOptions.SectionName).Get<AuditServiceOptions>()
                          ?? new AuditServiceOptions();
            client.BaseAddress = new Uri(options.BaseUrl);
            client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
        });
        
        // Authentication and authorization services.
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IAuthorizationService, AuthorizationService>();
        services.AddScoped<IPolicyService, PolicyService>();

        // Repositories base (AddScoped for per-request lifetime)
        services.AddScoped<IAddressRepository, AddressRepository>();
        services.AddScoped<IAddressTypeRepository, AddressTypeRepository>();
        services.AddScoped<IBankInformationRepository, BankInformationRepository>();
        services.AddScoped<IBranchOfficeRepository, BranchOfficeRepository>();
        services.AddScoped<IBranchOfficeAddressRepository, BranchOfficeAddressRepository>();
        services.AddScoped<ICreditAccountRepository, CreditAccountRepository>();
        services.AddScoped<ICreditTransactionRepository, CreditTransactionRepository>();
        services.AddScoped<ICorporateCustomerRepository, CorporateCustomerRepository>();
        services.AddScoped<ICountryRepository, CountryRepository>();
        services.AddScoped<ICustomerAddressRepository, CustomerAddressRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IIdentificationTypeRepository, IdentificationTypeRepository>();
        services.AddScoped<IIndividualCustomerRepository, IndividualCustomerRepository>();
        services.AddScoped<IStatusRepository, StatusRepository>();
        services.AddScoped<ITaxInformationAddressRepository, TaxInformationAddressRepository>();
        services.AddScoped<ITaxInformationRepository, TaxInformationRepository>();

        // Register Handlers or Infrastructure-specific services (if any, e.g. messaging services, file storage, etc.)
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(AssemblyReference).Assembly));

        return services;
    }
}

