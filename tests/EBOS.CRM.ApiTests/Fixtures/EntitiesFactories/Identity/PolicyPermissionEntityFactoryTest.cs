using EBOS.CRM.Domain.Entities.Identity;

namespace EBOS.CRM.ApiTests.Fixtures.EntitiesFactories.Identity;

public class PolicyPermissionEntityFactoryTest
{
    private static PolicyPermission CreateValidPolicyPermission(long policyId = 1, long permissionId = 2)
    {
        return new PolicyPermission
        {
            PolicyId = policyId,
            PermissionId = permissionId,
            AssignedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = 1
        };
    }

    [Fact]
    public void CreateValidPolicyPermission_Defaults_AreSet()
    {
        var entity = CreateValidPolicyPermission();

        Assert.NotNull(entity);
        Assert.Equal(1, entity.PolicyId);
        Assert.Equal(2, entity.PermissionId);
    }

    [Fact]
    public void CreateValidPolicyPermission_CustomValues_AreApplied()
    {
        var entity = CreateValidPolicyPermission(policyId: 10, permissionId: 20);

        Assert.Equal(10, entity.PolicyId);
        Assert.Equal(20, entity.PermissionId);
    }
}
