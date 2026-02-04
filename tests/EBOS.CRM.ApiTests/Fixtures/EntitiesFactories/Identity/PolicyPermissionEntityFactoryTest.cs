using EBOS.CRM.Domain.Entities.Identity;

namespace EBOS.CRM.ApiTests.Fixtures.EntitiesFactories.Identity;

public class PolicyPermissionEntityFactoryTest
{
    public static PolicyPermission CreateValidPolicyPermission(
        long policyId = 1,
        long permissionId = 2,
        DateTime? assignedAt = null)
    {
        return new PolicyPermission
        {
            PolicyId = policyId,
            PermissionId = permissionId,
            AssignedAt = assignedAt ?? new DateTime(2025, 01, 01, 0, 0, 0, DateTimeKind.Utc)
        };
    }

    [Fact]
    public void CreateValidPolicyPermission_Defaults_AreSet()
    {
        var entity = CreateValidPolicyPermission();

        Assert.NotNull(entity);
        Assert.Equal(1, entity.PolicyId);
        Assert.Equal(2, entity.PermissionId);
        Assert.Equal(new DateTime(2025, 01, 01, 0, 0, 0, DateTimeKind.Utc), entity.AssignedAt);
    }

    [Fact]
    public void CreateValidPolicyPermission_CustomValues_AreApplied()
    {
        var assignedAt = new DateTime(2025, 06, 01, 12, 0, 0, DateTimeKind.Utc);
        var entity = CreateValidPolicyPermission(
            policyId: 10,
            permissionId: 20,
            assignedAt: assignedAt);

        Assert.Equal(10, entity.PolicyId);
        Assert.Equal(20, entity.PermissionId);
        Assert.Equal(assignedAt, entity.AssignedAt);
    }
}
