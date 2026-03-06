using System.Reflection;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using EBOS.CRM.Application.Shared.Commands;
using EBOS.CRM.Application.Behavior;
using EBOS.CRM.Application.Features.CRM.Opportunity.Services;
using EBOS.CRM.Application.Features.CRM.Lead.Services;
using EBOS.CRM.Application.Features.CRM.Quote.Services;
using EBOS.CRM.Application.Features.CRM.Customer.Services;
using EBOS.CRM.Application.Features.CRM.BranchOfficeAddress.Services;
using EBOS.CRM.Application.Features.CRM.Address.Services;
using EBOS.CRM.Application.Features.CRM.BankInformation.Services;
using EBOS.CRM.Application.Features.CRM.BranchOffice.Services;
using EBOS.CRM.Application.Features.CRM.CustomerAddress.Services;
using EBOS.CRM.Application.Features.CRM.IndividualCustomer.Services;
using EBOS.CRM.Application.Features.CRM.CustomerConsent.Services;
using EBOS.CRM.Application.Features.CRM.CustomerPreference.Services;
using EBOS.CRM.Application.Features.CRM.Service.Case.Services;
using EBOS.CRM.Application.Features.CRM.Service.Queue.Services;
using EBOS.CRM.Application.Features.CRM.Service.Sla.Services;
using EBOS.CRM.Application.Features.CRM.CustomerPrivacy;
using EBOS.CRM.Application.Options;
using EBOS.CRM.Application.Shared.Observability;
using EBOS.CRM.Domain.Interfaces.Services.CRM;

namespace EBOS.CRM.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Registers all Handlers (Commands and Queries)
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CurrentUserContextBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PolicyAuthorizationBehavior<,>));

        services.AddOptions<CommandExecutionOptions>();
        services.AddOptions<CaseWorkflowOptions>();
        services.AddScoped<CustomerPrivacyExecutionService>();
        services.AddScoped<CustomerPrivacyRetentionService>();
        services.AddScoped<ICommandExecutionPipeline, CommandExecutionPipeline>();
        services.AddScoped<IDomainOperationalEventPublisher, DomainOperationalEventPublisher>();
        services.AddScoped<IOpportunityStageValidationService, OpportunityStageValidationService>();
        services.AddScoped<ILeadConversionValidationService, LeadConversionValidationService>();
        services.AddScoped<IQuoteOpportunityValidationService, QuoteOpportunityValidationService>();
        services.AddScoped<ICustomerReferenceValidationService, CustomerReferenceValidationService>();
        services.AddScoped<IBranchOfficeAddressReferenceValidationService, BranchOfficeAddressReferenceValidationService>();
        services.AddScoped<IAddressReferenceValidationService, AddressReferenceValidationService>();
        services.AddScoped<IBankInformationReferenceValidationService, BankInformationReferenceValidationService>();
        services.AddScoped<IBranchOfficeReferenceValidationService, BranchOfficeReferenceValidationService>();
        services.AddScoped<ICustomerAddressReferenceValidationService, CustomerAddressReferenceValidationService>();
        services.AddScoped<IIndividualCustomerReferenceValidationService, IndividualCustomerReferenceValidationService>();
        services.AddScoped<ICustomerConsentValidationService, CustomerConsentValidationService>();
        services.AddScoped<ICustomerPreferenceValidationService, CustomerPreferenceValidationService>();
        services.AddScoped<ICaseReferenceValidationService, CaseReferenceValidationService>();
        services.AddScoped<IQueueOperationalValidationService, QueueOperationalValidationService>();
        services.AddScoped<ISlaOperationalValidationService, SlaOperationalValidationService>();

        return services;
    }

    public static IServiceCollection AddApplicationMappings(this IServiceCollection services)
    {
        // Create and register the configuration
        var mapsterConfig = new TypeAdapterConfig();

        var asm = Assembly.GetExecutingAssembly();
        // We register the mappings (those we already have in Mapping/*.cs)
        var registerTypes = asm.GetTypes()
            .Where(t =>
                t is { IsClass: true, IsAbstract: false } &&
                t.Name.StartsWith("Mapping", StringComparison.Ordinal) &&
                typeof(IRegister).IsAssignableFrom(t));

        foreach (var t in registerTypes)
        {
            var reg = (IRegister)Activator.CreateInstance(t)!;
            reg.Register(mapsterConfig);
        }

        // Dependency Injection
        services.AddSingleton(mapsterConfig);
        services.AddScoped<IMapper, ServiceMapper>();

        return services;
    }
}

