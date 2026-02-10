using EBOS.CRM.Domain.Entities.Identity;

namespace EBOS.CRM.ApiTests.Fixtures.EntitiesFactories.Identity;

public class RoleEntityFactoryTest
{
    private static Role CreateValidRole(string code = "CRM_USER", string name = "CRM User",
        bool isSystem = false, bool isActive = true)
    {
        return new Role
        {
            Code = code,
            Name = name,
            Description = "Test role",
            IsSystem = isSystem,
            IsActive = isActive
        };
    }

    [Fact]
    public void CreateValidRole_Defaults_AreSet()
    {
        var role = CreateValidRole();

        Assert.NotNull(role);
        Assert.Equal("CRM_USER", role.Code);
        Assert.Equal("CRM User", role.Name);
        Assert.False(role.IsSystem);
        Assert.True(role.IsActive);
    }

    [Fact]
    public void CreateValidRole_CustomValues_AreApplied()
    {
        var role = CreateValidRole(code: "CRM_ADMIN", name: "CRM Admin", isSystem: true, isActive: false);

        Assert.Equal("CRM_ADMIN", role.Code);
        Assert.Equal("CRM Admin", role.Name);
        Assert.True(role.IsSystem);
        Assert.False(role.IsActive);
    }
}
