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

        // Scan all classes that implement IRegister
        config.Scan(AppDomain.CurrentDomain.GetAssemblies());

        // Compiles in fail-fast mode: if something goes wrong, throws an exception
        var ex = Record.Exception(() => config.Compile(failFast: true));

        Assert.Null(ex);
    }
}
