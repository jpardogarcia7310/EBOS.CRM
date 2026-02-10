using EBOS.CRM.Domain.Entities.Identity;

namespace EBOS.CRM.ApiTests.Fixtures.EntitiesFactories.Identity;

public class PolicyRuleEntityFactoryTest
{
    private static PolicyRule CreateValidPolicyRule(long policyId = 1, string name = "Default rule",
        string effect = "Permit", int priority = 1, bool isActive = true)
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
        var rule = CreateValidPolicyRule();

        Assert.NotNull(rule);
        Assert.Equal(1, rule.PolicyId);
        Assert.Equal("Default rule", rule.Name);
        Assert.Equal("Permit", rule.Effect);
        Assert.Equal(1, rule.Priority);
        Assert.True(rule.IsActive);
    }

    [Fact]
    public void CreateValidPolicyRule_CustomValues_AreApplied()
    {
        var rule = CreateValidPolicyRule(policyId: 10, name: "Deny rule", effect: "Deny", priority: 5, isActive: false);

        Assert.Equal(10, rule.PolicyId);
        Assert.Equal("Deny rule", rule.Name);
        Assert.Equal("Deny", rule.Effect);
        Assert.Equal(5, rule.Priority);
        Assert.False(rule.IsActive);
    }
}
