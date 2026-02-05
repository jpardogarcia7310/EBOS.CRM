using EBOS.CRM.Domain.Entities.Identity;

namespace EBOS.CRM.ApiTests.Fixtures.EntitiesFactories.Identity;

public class AbacAttributeEntityFactoryTest
{
    public static AbacAttribute CreateValidAbacAttribute(
        string code = "subject.department",
        string name = "Department",
        string category = "Subject",
        string dataType = "String",
        string? description = "User department",
        bool isActive = true)
    {
        return new AbacAttribute
        {
            Code = code,
            Name = name,
            Category = category,
            DataType = dataType,
            Description = description,
            IsActive = isActive
        };
    }

    [Fact]
    public void CreateValidAbacAttribute_Defaults_AreSet()
    {
        var entity = CreateValidAbacAttribute();

        Assert.NotNull(entity);
        Assert.Equal("subject.department", entity.Code);
        Assert.Equal("Department", entity.Name);
        Assert.Equal("Subject", entity.Category);
        Assert.Equal("String", entity.DataType);
        Assert.Equal("User department", entity.Description);
        Assert.True(entity.IsActive);
    }

    [Fact]
    public void CreateValidAbacAttribute_CustomValues_AreApplied()
    {
        var entity = CreateValidAbacAttribute(
            code: "resource.type",
            name: "ResourceType",
            category: "Resource",
            dataType: "Number",
            description: "Type code",
            isActive: false);

        Assert.Equal("resource.type", entity.Code);
        Assert.Equal("ResourceType", entity.Name);
        Assert.Equal("Resource", entity.Category);
        Assert.Equal("Number", entity.DataType);
        Assert.Equal("Type code", entity.Description);
        Assert.False(entity.IsActive);
    }
}
