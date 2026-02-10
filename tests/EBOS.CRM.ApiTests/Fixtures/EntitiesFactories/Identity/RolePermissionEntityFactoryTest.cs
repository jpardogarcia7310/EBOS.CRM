using EBOS.CRM.Domain.Entities.Identity;

namespace EBOS.CRM.ApiTests.Fixtures.EntitiesFactories.Identity;

public class RolePermissionEntityFactoryTest
{
    private static RolePermission CreateValidRolePermission(long roleId = 1, long permissionId = 2)
    {
        return new RolePermission
        {
            RoleId = roleId,
            PermissionId = permissionId,
            AssignedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = 1
        };
    }

    [Fact]
    public void CreateValidRolePermission_Defaults_AreSet()
    {
        var entity = CreateValidRolePermission();

        Assert.NotNull(entity);
        Assert.Equal(1, entity.RoleId);
        Assert.Equal(2, entity.PermissionId);
    }

    [Fact]
    public void CreateValidRolePermission_CustomValues_AreApplied()
    {
        var entity = CreateValidRolePermission(roleId: 10, permissionId: 20);

        Assert.Equal(10, entity.RoleId);
        Assert.Equal(20, entity.PermissionId);
    }
}
