using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace EBOS.CRM.Application;

public static partial class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Registra todos los Handlers (Commands y Queries)
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });

        return services;
    }

    public static IServiceCollection AddApplicattionMappings(this IServiceCollection services)
    {
        // Crear y registrar la configuracion
        var mapsterConfig = new TypeAdapterConfig();

        var asm = Assembly.GetExecutingAssembly();
        // Registramos los Mapeos (los que ya tenemos en Mapping/*.cs)
        var registerTypes = asm.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.Name.StartsWith("Mapping", StringComparison.Ordinal) && typeof(IRegister).IsAssignableFrom(t));
        foreach (var t in registerTypes)
        {
            var reg = (IRegister)Activator.CreateInstance(t)!;
            reg.Register(mapsterConfig);
        }

        // Inyección de dependencias
        services.AddSingleton(mapsterConfig);
        services.AddScoped<IMapper, ServiceMapper>();

        return services;
    }
}
