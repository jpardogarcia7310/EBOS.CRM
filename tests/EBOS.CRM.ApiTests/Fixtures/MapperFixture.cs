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
        // Scan explicit application assembly to avoid missing mappings in isolated test runs.
        config.Scan(AppDomain.CurrentDomain.GetAssemblies());
        config.Scan(typeof(EBOS.CRM.Application.Mappings.CRM.MappingLead).Assembly);

        // Compile the configuration
        config.Compile();

        // Build the mapper, passing null as IServiceProvider
        Mapper = new ServiceMapper(null, config);
    }
}


