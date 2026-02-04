using EBOS.CRM.Domain.Entities.Identity;

namespace EBOS.CRM.ApiTests.Fixtures.EntitiesFactories.Identity;

public class RolePermissionEntityFactoryTest
{
    public static RolePermission CreateValidRolePermission(
        long roleId = 1,
        long permissionId = 2,
        DateTime? assignedAt = null)
    {
        return new RolePermission
        {
            RoleId = roleId,
            PermissionId = permissionId,
            AssignedAt = assignedAt ?? new DateTime(2025, 01, 01, 0, 0, 0, DateTimeKind.Utc)
        };
    }

    [Fact]
    public void CreateValidRolePermission_Defaults_AreSet()
    {
        var entity = CreateValidRolePermission();

        Assert.NotNull(entity);
        Assert.Equal(1, entity.RoleId);
        Assert.Equal(2, entity.PermissionId);
        Assert.Equal(new DateTime(2025, 01, 01, 0, 0, 0, DateTimeKind.Utc), entity.AssignedAt);
    }

    [Fact]
    public void CreateValidRolePermission_CustomValues_AreApplied()
    {
        var assignedAt = new DateTime(2025, 06, 01, 12, 0, 0, DateTimeKind.Utc);
        var entity = CreateValidRolePermission(
            roleId: 10,
            permissionId: 20,
            assignedAt: assignedAt);

        Assert.Equal(10, entity.RoleId);
        Assert.Equal(20, entity.PermissionId);
        Assert.Equal(assignedAt, entity.AssignedAt);
    }
}
