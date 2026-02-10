using EBOS.CRM.Domain.Entities.Identity;

namespace EBOS.CRM.ApiTests.Fixtures.EntitiesFactories.Identity;

public class UserPolicyEntityFactoryTest
{
    private static UserPolicy CreateValidUserPolicy(long userId = 1, long policyId = 2)
    {
        return new UserPolicy
        {
            UserId = userId,
            PolicyId = policyId,
            AssignedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = 1
        };
    }

    [Fact]
    public void CreateValidUserPolicy_Defaults_AreSet()
    {
        var entity = CreateValidUserPolicy();

        Assert.NotNull(entity);
        Assert.Equal(1, entity.UserId);
        Assert.Equal(2, entity.PolicyId);
    }

    [Fact]
    public void CreateValidUserPolicy_CustomValues_AreApplied()
    {
        var entity = CreateValidUserPolicy(userId: 10, policyId: 20);

        Assert.Equal(10, entity.UserId);
        Assert.Equal(20, entity.PolicyId);
    }
}
