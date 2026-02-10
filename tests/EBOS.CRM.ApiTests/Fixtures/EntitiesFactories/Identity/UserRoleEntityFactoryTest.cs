using EBOS.CRM.Domain.Entities.Identity;

namespace EBOS.CRM.ApiTests.Fixtures.EntitiesFactories.Identity;

public class UserRoleEntityFactoryTest
{
    private static UserRole CreateValidUserRole(long userId = 1, long roleId = 2)
    {
        return new UserRole
        {
            UserId = userId,
            RoleId = roleId,
            AssignedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = 1
        };
    }

    [Fact]
    public void CreateValidUserRole_Defaults_AreSet()
    {
        var entity = CreateValidUserRole();

        Assert.NotNull(entity);
        Assert.Equal(1, entity.UserId);
        Assert.Equal(2, entity.RoleId);
    }

    [Fact]
    public void CreateValidUserRole_CustomValues_AreApplied()
    {
        var entity = CreateValidUserRole(userId: 10, roleId: 20);

        Assert.Equal(10, entity.UserId);
        Assert.Equal(20, entity.RoleId);
    }
}
