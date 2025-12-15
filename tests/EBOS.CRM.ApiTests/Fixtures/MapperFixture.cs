using Mapster;
using MapsterMapper;

namespace EBOS.CRM.ApiTests.Fixtures;

public class MapperFixture
{
    public IMapper Mapper { get; }

    public MapperFixture()
    {
        // Crear configuración de Mapster
        var config = new TypeAdapterConfig();

        // Escanear automáticamente todas las clases que implementan IRegister (ej. MappingCountry)
        config.Scan(AppDomain.CurrentDomain.GetAssemblies());

        // Compilar la configuración
        config.Compile();

        // Construir el mapper pasando null como IServiceProvider
        Mapper = new ServiceMapper(null, config);
    }
}