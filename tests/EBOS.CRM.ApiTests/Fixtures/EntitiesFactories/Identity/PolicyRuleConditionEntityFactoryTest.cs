using EBOS.CRM.Domain.Entities.Identity;

namespace EBOS.CRM.ApiTests.Fixtures.EntitiesFactories.Identity;

public class PolicyRuleConditionEntityFactoryTest
{
    private static PolicyRuleCondition CreateValidPolicyRuleCondition(long policyRuleId = 1, long attributeId = 2,
        string @operator = "Equals", string value = "admin", string valueType = "String", bool isNegated = false)
    {
        return new PolicyRuleCondition
        {
            PolicyRuleId = policyRuleId,
            AttributeId = attributeId,
            Operator = @operator,
            Value = value,
            ValueType = valueType,
            IsNegated = isNegated
        };
    }

    [Fact]
    public void CreateValidPolicyRuleCondition_Defaults_AreSet()
    {
        var condition = CreateValidPolicyRuleCondition();

        Assert.NotNull(condition);
        Assert.Equal(1, condition.PolicyRuleId);
        Assert.Equal(2, condition.AttributeId);
        Assert.Equal("Equals", condition.Operator);
        Assert.Equal("admin", condition.Value);
        Assert.Equal("String", condition.ValueType);
        Assert.False(condition.IsNegated);
    }

    [Fact]
    public void CreateValidPolicyRuleCondition_CustomValues_AreApplied()
    {
        var condition = CreateValidPolicyRuleCondition(policyRuleId: 10, attributeId: 20, @operator: "Contains",
            value: "sales", valueType: "String", isNegated: true);

        Assert.Equal(10, condition.PolicyRuleId);
        Assert.Equal(20, condition.AttributeId);
        Assert.Equal("Contains", condition.Operator);
        Assert.Equal("sales", condition.Value);
        Assert.Equal("String", condition.ValueType);
        Assert.True(condition.IsNegated);
    }
}
