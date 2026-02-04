using EBOS.CRM.Domain.Entities.Identity;

namespace EBOS.CRM.ApiTests.Fixtures.EntitiesFactories.Identity;

public class PolicyRoleEntityFactoryTest
{
    public static PolicyRole CreateValidPolicyRole(
        long policyId = 1,
        long roleId = 2,
        DateTime? assignedAt = null)
    {
        return new PolicyRole
        {
            PolicyId = policyId,
            RoleId = roleId,
            AssignedAt = assignedAt ?? new DateTime(2025, 01, 01, 0, 0, 0, DateTimeKind.Utc)
        };
    }

    [Fact]
    public void CreateValidPolicyRole_Defaults_AreSet()
    {
        var entity = CreateValidPolicyRole();

        Assert.NotNull(entity);
        Assert.Equal(1, entity.PolicyId);
        Assert.Equal(2, entity.RoleId);
        Assert.Equal(new DateTime(2025, 01, 01, 0, 0, 0, DateTimeKind.Utc), entity.AssignedAt);
    }

    [Fact]
    public void CreateValidPolicyRole_CustomValues_AreApplied()
    {
        var assignedAt = new DateTime(2025, 06, 01, 12, 0, 0, DateTimeKind.Utc);
        var entity = CreateValidPolicyRole(
            policyId: 10,
            roleId: 20,
            assignedAt: assignedAt);

        Assert.Equal(10, entity.PolicyId);
        Assert.Equal(20, entity.RoleId);
        Assert.Equal(assignedAt, entity.AssignedAt);
    }
}
