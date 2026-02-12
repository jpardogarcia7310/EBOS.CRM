using System.Reflection;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using EBOS.CRM.Application.Services.Commands;
using EBOS.CRM.Application.Behavior;
using EBOS.CRM.Application.Options;
using EBOS.CRM.Application.Services.CRM;
using EBOS.CRM.Application.Services.Interfaces;
using EBOS.CRM.Domain.Interfaces.Services;

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
        services.AddScoped<ICommandExecutionPipeline, CommandExecutionPipeline>();
        services.AddScoped<ICaseRoutingService, CaseRoutingService>();
        services.AddScoped<ICaseWorkflowService, CaseWorkflowService>();

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

