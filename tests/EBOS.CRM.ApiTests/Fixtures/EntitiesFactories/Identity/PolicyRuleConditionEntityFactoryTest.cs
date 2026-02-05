using EBOS.CRM.Domain.Entities.Identity;

namespace EBOS.CRM.ApiTests.Fixtures.EntitiesFactories.Identity;

public class PolicyRuleConditionEntityFactoryTest
{
    public static PolicyRuleCondition CreateValidPolicyRuleCondition(
        long policyRuleId = 1,
        long attributeId = 2,
        string op = "Equals",
        string value = "Sales",
        string valueType = "String",
        bool isNegated = false)
    {
        return new PolicyRuleCondition
        {
            PolicyRuleId = policyRuleId,
            AttributeId = attributeId,
            Operator = op,
            Value = value,
            ValueType = valueType,
            IsNegated = isNegated
        };
    }

    [Fact]
    public void CreateValidPolicyRuleCondition_Defaults_AreSet()
    {
        var entity = CreateValidPolicyRuleCondition();

        Assert.NotNull(entity);
        Assert.Equal(1, entity.PolicyRuleId);
        Assert.Equal(2, entity.AttributeId);
        Assert.Equal("Equals", entity.Operator);
        Assert.Equal("Sales", entity.Value);
        Assert.Equal("String", entity.ValueType);
        Assert.False(entity.IsNegated);
    }

    [Fact]
    public void CreateValidPolicyRuleCondition_CustomValues_AreApplied()
    {
        var entity = CreateValidPolicyRuleCondition(
            policyRuleId: 10,
            attributeId: 20,
            op: "In",
            value: "IT",
            valueType: "String",
            isNegated: true);

        Assert.Equal(10, entity.PolicyRuleId);
        Assert.Equal(20, entity.AttributeId);
        Assert.Equal("In", entity.Operator);
        Assert.Equal("IT", entity.Value);
        Assert.Equal("String", entity.ValueType);
        Assert.True(entity.IsNegated);
    }
}
