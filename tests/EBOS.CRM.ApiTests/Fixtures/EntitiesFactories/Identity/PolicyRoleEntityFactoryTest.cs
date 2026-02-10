using EBOS.CRM.Domain.Entities.Identity;

namespace EBOS.CRM.ApiTests.Fixtures.EntitiesFactories.Identity;

public class PolicyRoleEntityFactoryTest
{
    private static PolicyRole CreateValidPolicyRole(long policyId = 1, long roleId = 2)
    {
        return new PolicyRole
        {
            PolicyId = policyId,
            RoleId = roleId,
            AssignedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = 1
        };
    }

    [Fact]
    public void CreateValidPolicyRole_Defaults_AreSet()
    {
        var entity = CreateValidPolicyRole();

        Assert.NotNull(entity);
        Assert.Equal(1, entity.PolicyId);
        Assert.Equal(2, entity.RoleId);
    }

    [Fact]
    public void CreateValidPolicyRole_CustomValues_AreApplied()
    {
        var entity = CreateValidPolicyRole(policyId: 10, roleId: 20);

        Assert.Equal(10, entity.PolicyId);
        Assert.Equal(20, entity.RoleId);
    }
}
