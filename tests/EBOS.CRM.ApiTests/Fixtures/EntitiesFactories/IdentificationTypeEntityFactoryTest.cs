using EBOS.CRM.ApiTests.Fixtures.EntitiesFactories;
using EBOS.CRM.Domain.Entities;

namespace EBOS.CRM.ApiTests.Fixtures.EntitiesFactories;

public class IdentificationTypeEntityFactoryTest
{
    public static IdentificationType CreateValidIdentificationType(string code = "DNI", string description = "Documento")
    {
        return new IdentificationType
        {
            Code = code,
            Description = description
        };
    }
    [Fact]
    public void CreateValidIdentificationType_Defaults_AreSet()
    {
        var idType = CreateValidIdentificationType();

        Assert.NotNull(idType);
        Assert.Equal("DNI", idType.Code);
        Assert.Equal("Documento", idType.Description);
    }

    [Fact]
    public void CreateValidIdentificationType_CustomValues_AreApplied()
    {
        var idType = CreateValidIdentificationType(code: "PASS", description: "Passport");

        Assert.Equal("PASS", idType.Code);
        Assert.Equal("Passport", idType.Description);
    }
}


