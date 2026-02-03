using Mapster;
using MapsterMapper;

namespace EBOS.CRM.ApiTests.Fixtures;

public class MapperFixture
{
    public IMapper Mapper { get; }

    public MapperFixture()
    {
        // Create Mapster configuration
        var config = new TypeAdapterConfig();

        // Automatically scan all classes that implement IRegister (e.g., MappingCountry)
        config.Scan(AppDomain.CurrentDomain.GetAssemblies());

        // Compile the configuration
        config.Compile();

        // Build the mapper, passing null as IServiceProvider
        Mapper = new ServiceMapper(null, config);
    }
}
