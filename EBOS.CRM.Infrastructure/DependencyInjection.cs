using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;
using EBOS.CRM.Domain.Interfaces.Services;
using EBOS.CRM.Domain.Interfaces.Services.CRM;
using EBOS.CRM.Domain.Interfaces.Services.EBOS;
using EBOS.CRM.Domain.Interfaces.Services.Identity;
using EBOS.CRM.Infrastructure.Options;
using EBOS.CRM.Infrastructure.Repositories.Concrete.CRM;
using EBOS.CRM.Infrastructure.Repositories.Concrete.EBOS;
using EBOS.CRM.Infrastructure.Services.Audit;
using EBOS.CRM.Infrastructure.Services.Security;
using EBOS.CRM.Infrastructure.Services.CRM;
using EBOS.CRM.Infrastructure.Services.Lookup;
using Microsoft.Extensions.Configuration;

namespace EBOS.CRM.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMemoryCache();

        // DbContext registration
        services.AddDbContext<CrmDbContext>((sp, options) =>
        {
            var tenantContext = sp.GetService<ITenantContext>();
            var multiTenantOptions = sp.GetService<IOptions<MultiTenantOptions>>()?.Value
                                     ?? new MultiTenantOptions();
            var baseConnectionString = configuration.GetConnectionString("CrmConnection")
                                       ?? string.Empty;
            var connectionString = ResolveConnectionString(baseConnectionString, tenantContext, multiTenantOptions);

            options.UseSqlServer(connectionString);
        });

        services.AddOptions<AuditServiceOptions>()
            .Bind(configuration.GetSection(AuditServiceOptions.SectionName));
        services.AddOptions<CustomerDedupeOptions>()
            .Bind(configuration.GetSection(CustomerDedupeOptions.SectionName));
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
        services.AddScoped<IAccountHierarchyCycleGuard, AccountHierarchyCycleGuard>();
        services.AddScoped<IAddressRepository, AddressRepository>();
        services.AddScoped<IAddressTypeRepository, AddressTypeRepository>();
        services.AddScoped<IBankInformationRepository, BankInformationRepository>();
        services.AddScoped<IBranchOfficeRepository, BranchOfficeRepository>();
        services.AddScoped<IBranchOfficeAddressRepository, BranchOfficeAddressRepository>();
        services.AddScoped<ICaseActivityRepository, CaseActivityRepository>();
        services.AddScoped<ICaseRepository, CaseRepository>();
        services.AddScoped<ICaseRoutingService, CaseRoutingService>();
        services.AddScoped<ICaseWorkflowService, CaseWorkflowService>();
        services.AddScoped<ICustomerDedupeNormalizationService, CustomerDedupeNormalizationService>();
        services.AddScoped<ICreditAccountRepository, CreditAccountRepository>();
        services.AddScoped<ICreditTransactionRepository, CreditTransactionRepository>();
        services.AddScoped<ICorporateCustomerRepository, CorporateCustomerRepository>();
        services.AddScoped<ICountryRepository, CountryRepository>();
        services.AddScoped<IAccountContactRepository, AccountContactRepository>();
        services.AddScoped<IAccountContactRoleRepository, AccountContactRoleRepository>();
        services.AddScoped<IAccountHierarchyRepository, AccountHierarchyRepository>();
        services.AddScoped<ICustomerAddressRepository, CustomerAddressRepository>();
        services.AddScoped<ICustomerDedupeRepository, CustomerDedupeRepository>();
        services.AddScoped<ICustomerPreferenceRepository, CustomerPreferenceRepository>();
        services.AddScoped<ICustomerConsentRepository, CustomerConsentRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IIdentificationTypeRepository, IdentificationTypeRepository>();
        services.AddScoped<IIndividualCustomerRepository, IndividualCustomerRepository>();
        services.AddScoped<ILeadRepository, LeadRepository>();
        services.AddScoped<IOpportunityRepository, OpportunityRepository>();
        services.AddScoped<IOpportunityStageRepository, OpportunityStageRepository>();
        services.AddScoped<IQueueRepository, QueueRepository>();
        services.AddScoped<IQuoteRepository, QuoteRepository>();
        services.AddScoped<ISlaRepository, SlaRepository>();
        services.AddScoped<IStatusRepository, StatusRepository>();
        services.AddScoped<ITaxInformationAddressRepository, TaxInformationAddressRepository>();
        services.AddScoped<ITaxInformationRepository, TaxInformationRepository>();
        services.AddScoped<ITenantConfigurationRepository, TenantConfigurationRepository>();
        services.AddScoped<ITenantQuotaRepository, TenantQuotaRepository>();
        services.AddScoped<ITenantUsageMetricRepository, TenantUsageMetricRepository>();
        services.AddScoped<IAddressTypeRepository, AddressTypeRepository>();
        services.AddScoped<ICountryRepository, CountryRepository>();
        services.AddScoped<IIdentificationTypeRepository, IdentificationTypeRepository>();
        services.AddScoped<IStatusRepository, StatusRepository>();
        services.AddScoped<ILeadRepository, LeadRepository>();
        services.AddScoped<IOpportunityRepository, OpportunityRepository>();
        services.AddScoped<IOpportunityStageRepository, OpportunityStageRepository>();
        services.AddScoped<IQuoteRepository, QuoteRepository>();
        services.AddScoped<ILeadDebtorCheckService, LeadDebtorCheckService>();
        services.AddScoped<ILookupNormalizationService, LookupNormalizationService>();
        services.AddScoped<ILookupSeedService, LookupSeedService>();

        // Register Handlers or Infrastructure-specific services (if any, e.g. messaging services, file storage, etc.)
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(AssemblyReference).Assembly));

        return services;
    }

    private static string ResolveConnectionString(string baseConnectionString, ITenantContext? tenantContext,
        MultiTenantOptions options)
    {
        var tenantId = tenantContext?.TenantId ?? 0;

        if (options.Strategy != MultiTenantStrategy.Database || tenantId <= 0)
        {
            return baseConnectionString;
        }

        if (string.IsNullOrWhiteSpace(options.ConnectionStringTemplate))
        {
            throw new InvalidOperationException(
                "MultiTenant:ConnectionStringTemplate is required for Database strategy.");
        }

        var template = options.ConnectionStringTemplate!;

        if (!template.Contains("{tenantId}", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "MultiTenant:ConnectionStringTemplate must include '{tenantId}'.");
        }

        return template.Replace("{tenantId}", tenantId.ToString(),
            StringComparison.OrdinalIgnoreCase);
    }
}

