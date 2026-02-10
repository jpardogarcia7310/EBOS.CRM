using EBOS.CRM.Domain.Entities.Identity;

namespace EBOS.CRM.ApiTests.Fixtures.EntitiesFactories.Identity;

public class AbacAttributeEntityFactoryTest
{
    private static AbacAttribute CreateValidAbacAttribute(string code = "SUBJECT_ID", string name = "Subject Id",
        string category = "Subject", string dataType = "String", bool isActive = true)
    {
        return new AbacAttribute
        {
            Code = code,
            Name = name,
            Category = category,
            DataType = dataType,
            Description = "Test attribute",
            IsActive = isActive
        };
    }

    [Fact]
    public void CreateValidAbacAttribute_Defaults_AreSet()
    {
        var attribute = CreateValidAbacAttribute();

        Assert.NotNull(attribute);
        Assert.Equal("SUBJECT_ID", attribute.Code);
        Assert.Equal("Subject Id", attribute.Name);
        Assert.Equal("Subject", attribute.Category);
        Assert.Equal("String", attribute.DataType);
        Assert.True(attribute.IsActive);
    }

    [Fact]
    public void CreateValidAbacAttribute_CustomValues_AreApplied()
    {
        var attribute = CreateValidAbacAttribute(code: "ROLE", name: "Role", category: "Subject", dataType: "String",
            isActive: false);

        Assert.Equal("ROLE", attribute.Code);
        Assert.Equal("Role", attribute.Name);
        Assert.Equal("Subject", attribute.Category);
        Assert.Equal("String", attribute.DataType);
        Assert.False(attribute.IsActive);
    }
}
