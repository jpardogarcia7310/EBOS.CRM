using EBOS.CRM.Domain.Entities.Identity;

namespace EBOS.CRM.ApiTests.Fixtures.EntitiesFactories.Identity;

public class PolicyEntityFactoryTest
{
    public static Policy CreateValidPolicy(
        string code = "crm.customer.access",
        string name = "Customer Access",
        string? description = "Customer access policy",
        bool isSystem = true,
        bool isActive = true)
    {
        return new Policy
        {
            Code = code,
            Name = name,
            Description = description,
            IsSystem = isSystem,
            IsActive = isActive
        };
    }

    [Fact]
    public void CreateValidPolicy_Defaults_AreSet()
    {
        var entity = CreateValidPolicy();

        Assert.NotNull(entity);
        Assert.Equal("crm.customer.access", entity.Code);
        Assert.Equal("Customer Access", entity.Name);
        Assert.Equal("Customer access policy", entity.Description);
        Assert.True(entity.IsSystem);
        Assert.True(entity.IsActive);
    }

    [Fact]
    public void CreateValidPolicy_CustomValues_AreApplied()
    {
        var entity = CreateValidPolicy(
            code: "crm.customer.edit",
            name: "Customer Edit",
            description: "Edit customers",
            isSystem: false,
            isActive: false);

        Assert.Equal("crm.customer.edit", entity.Code);
        Assert.Equal("Customer Edit", entity.Name);
        Assert.Equal("Edit customers", entity.Description);
        Assert.False(entity.IsSystem);
        Assert.False(entity.IsActive);
    }
}
