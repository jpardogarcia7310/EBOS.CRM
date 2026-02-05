using EBOS.CRM.Domain.Entities.Identity;

namespace EBOS.CRM.ApiTests.Fixtures.EntitiesFactories.Identity;

public class UserPolicyEntityFactoryTest
{
    public static UserPolicy CreateValidUserPolicy(
        long userId = 1,
        long policyId = 2,
        DateTime? assignedAt = null)
    {
        return new UserPolicy
        {
            UserId = userId,
            PolicyId = policyId,
            AssignedAt = assignedAt ?? new DateTime(2025, 01, 01, 0, 0, 0, DateTimeKind.Utc)
        };
    }

    [Fact]
    public void CreateValidUserPolicy_Defaults_AreSet()
    {
        var entity = CreateValidUserPolicy();

        Assert.NotNull(entity);
        Assert.Equal(1, entity.UserId);
        Assert.Equal(2, entity.PolicyId);
        Assert.Equal(new DateTime(2025, 01, 01, 0, 0, 0, DateTimeKind.Utc), entity.AssignedAt);
    }

    [Fact]
    public void CreateValidUserPolicy_CustomValues_AreApplied()
    {
        var assignedAt = new DateTime(2025, 06, 01, 12, 0, 0, DateTimeKind.Utc);
        var entity = CreateValidUserPolicy(
            userId: 10,
            policyId: 20,
            assignedAt: assignedAt);

        Assert.Equal(10, entity.UserId);
        Assert.Equal(20, entity.PolicyId);
        Assert.Equal(assignedAt, entity.AssignedAt);
    }
}
