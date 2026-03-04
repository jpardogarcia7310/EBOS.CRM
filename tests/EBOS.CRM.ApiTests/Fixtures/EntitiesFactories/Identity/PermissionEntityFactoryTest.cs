using EBOS.CRM.Domain.Entities.Identity;

namespace EBOS.CRM.ApiTests.Fixtures.EntitiesFactories.Identity;

public class PermissionEntityFactoryTest
{
    private static Permission CreateValidPermission(string code = "CRM_READ", string name = "Read CRM",
        bool isSystem = false)
    {
        return new Permission
        {
            Code = code,
            Name = name,
            Description = "Test permission",
            IsSystem = isSystem
        };
    }

    [Fact]
    public void CreateValidPermission_Defaults_AreSet()
    {
        var permission = CreateValidPermission();

        Assert.NotNull(permission);
        Assert.Equal("CRM_READ", permission.Code);
        Assert.Equal("Read CRM", permission.Name);
        Assert.False(permission.IsSystem);
    }

    [Fact]
    public void CreateValidPermission_CustomValues_AreApplied()
    {
        var permission = CreateValidPermission(code: "CRM_WRITE", name: "Write CRM", isSystem: true);

        Assert.Equal("CRM_WRITE", permission.Code);
        Assert.Equal("Write CRM", permission.Name);
        Assert.True(permission.IsSystem);
    }
}
