using EBOS.CRM.Domain.Entities.Identity;

namespace EBOS.CRM.ApiTests.Fixtures.EntitiesFactories.Identity;

public class PermissionEntityFactoryTest
{
    public static Permission CreateValidPermission(
        string code = "crm.customer.read",
        string name = "Read Customers",
        string? description = "Allows reading customers",
        bool isSystem = true)
    {
        return new Permission
        {
            Code = code,
            Name = name,
            Description = description,
            IsSystem = isSystem
        };
    }

    [Fact]
    public void CreateValidPermission_Defaults_AreSet()
    {
        var entity = CreateValidPermission();

        Assert.NotNull(entity);
        Assert.Equal("crm.customer.read", entity.Code);
        Assert.Equal("Read Customers", entity.Name);
        Assert.Equal("Allows reading customers", entity.Description);
        Assert.True(entity.IsSystem);
    }

    [Fact]
    public void CreateValidPermission_CustomValues_AreApplied()
    {
        var entity = CreateValidPermission(
            code: "crm.customer.write",
            name: "Write Customers",
            description: "Allows writing customers",
            isSystem: false);

        Assert.Equal("crm.customer.write", entity.Code);
        Assert.Equal("Write Customers", entity.Name);
        Assert.Equal("Allows writing customers", entity.Description);
        Assert.False(entity.IsSystem);
    }
}
