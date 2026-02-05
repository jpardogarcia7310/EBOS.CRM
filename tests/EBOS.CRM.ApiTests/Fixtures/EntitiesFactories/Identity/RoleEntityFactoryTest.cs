using EBOS.CRM.Domain.Entities.Identity;

namespace EBOS.CRM.ApiTests.Fixtures.EntitiesFactories.Identity;

public class RoleEntityFactoryTest
{
    public static Role CreateValidRole(
        string code = "admin",
        string name = "Administrator",
        string? description = "System administrator",
        bool isSystem = true,
        bool isActive = true)
    {
        return new Role
        {
            Code = code,
            Name = name,
            Description = description,
            IsSystem = isSystem,
            IsActive = isActive
        };
    }

    [Fact]
    public void CreateValidRole_Defaults_AreSet()
    {
        var entity = CreateValidRole();

        Assert.NotNull(entity);
        Assert.Equal("admin", entity.Code);
        Assert.Equal("Administrator", entity.Name);
        Assert.Equal("System administrator", entity.Description);
        Assert.True(entity.IsSystem);
        Assert.True(entity.IsActive);
    }

    [Fact]
    public void CreateValidRole_CustomValues_AreApplied()
    {
        var entity = CreateValidRole(
            code: "user",
            name: "User",
            description: "Standard user",
            isSystem: false,
            isActive: false);

        Assert.Equal("user", entity.Code);
        Assert.Equal("User", entity.Name);
        Assert.Equal("Standard user", entity.Description);
        Assert.False(entity.IsSystem);
        Assert.False(entity.IsActive);
    }
}
