using EBOS.CRM.Domain.Entities.Identity;

namespace EBOS.CRM.ApiTests.Fixtures.EntitiesFactories.Identity;

public class PolicyEntityFactoryTest
{
    private static Policy CreateValidPolicy(string code = "CRM_DEFAULT", string name = "CRM Default",
        bool isSystem = false, bool isActive = true)
    {
        return new Policy
        {
            Code = code,
            Name = name,
            Description = "Test policy",
            IsSystem = isSystem,
            IsActive = isActive
        };
    }

    [Fact]
    public void CreateValidPolicy_Defaults_AreSet()
    {
        var policy = CreateValidPolicy();

        Assert.NotNull(policy);
        Assert.Equal("CRM_DEFAULT", policy.Code);
        Assert.Equal("CRM Default", policy.Name);
        Assert.False(policy.IsSystem);
        Assert.True(policy.IsActive);
    }

    [Fact]
    public void CreateValidPolicy_CustomValues_AreApplied()
    {
        var policy = CreateValidPolicy(code: "CRM_ADMIN", name: "CRM Admin", isSystem: true, isActive: false);

        Assert.Equal("CRM_ADMIN", policy.Code);
        Assert.Equal("CRM Admin", policy.Name);
        Assert.True(policy.IsSystem);
        Assert.False(policy.IsActive);
    }
}
