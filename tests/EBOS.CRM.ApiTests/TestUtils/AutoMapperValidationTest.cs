using EBOS.CRM.Application.Mappings;
using Mapster;

namespace EBOS.CRM.ApiTests.TestUtils;

public class MapsterValidationTest
{
    [Fact]
    public void Configuration_IsValid()
    {
        var config = new TypeAdapterConfig
        {
            RequireExplicitMapping = true,
            RequireDestinationMemberSource = true
        };

        // Escanea todas las clases que implementan IRegister (incluye MappingCountry)
        config.Scan(AppDomain.CurrentDomain.GetAssemblies());

        // Compila en modo fail-fast: si algo está mal, lanza excepción
        var ex = Record.Exception(() => config.Compile(failFast: true));

        Assert.Null(ex);
    }
}