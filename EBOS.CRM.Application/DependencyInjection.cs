using System.Reflection;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;

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

