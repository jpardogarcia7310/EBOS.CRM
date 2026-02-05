using EBOS.CRM.Domain.Entities.Identity;

namespace EBOS.CRM.ApiTests.Fixtures.EntitiesFactories.Identity;

public class PolicyRuleEntityFactoryTest
{
    public static PolicyRule CreateValidPolicyRule(
        long policyId = 1,
        string name = "Default rule",
        string effect = "Permit",
        int priority = 1,
        bool isActive = true)
    {
        return new PolicyRule
        {
            PolicyId = policyId,
            Name = name,
            Effect = effect,
            Priority = priority,
            IsActive = isActive
        };
    }

    [Fact]
    public void CreateValidPolicyRule_Defaults_AreSet()
    {
        var entity = CreateValidPolicyRule();

        Assert.NotNull(entity);
        Assert.Equal(1, entity.PolicyId);
        Assert.Equal("Default rule", entity.Name);
        Assert.Equal("Permit", entity.Effect);
        Assert.Equal(1, entity.Priority);
        Assert.True(entity.IsActive);
    }

    [Fact]
    public void CreateValidPolicyRule_CustomValues_AreApplied()
    {
        var entity = CreateValidPolicyRule(
            policyId: 10,
            name: "Deny rule",
            effect: "Deny",
            priority: 2,
            isActive: false);

        Assert.Equal(10, entity.PolicyId);
        Assert.Equal("Deny rule", entity.Name);
        Assert.Equal("Deny", entity.Effect);
        Assert.Equal(2, entity.Priority);
        Assert.False(entity.IsActive);
    }
}
