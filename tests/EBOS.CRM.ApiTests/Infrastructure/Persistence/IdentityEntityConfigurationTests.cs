using EBOS.CRM.Domain.Entities.Identity;
using EBOS.CRM.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EBOS.CRM.ApiTests.Infrastructure.Persistence;

public class IdentityEntityConfigurationTests
{
    private static CrmDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<CrmDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
            .Options;
        return new CrmDbContext(options);
    }

    [Fact]
    public void AbacAttribute_Config_Is_Correct()
    {
        using var context = CreateContext();
        var entity = GetEntityType<AbacAttribute>(context);

        AssertTable(entity, "AbacAttributes", "IAM");
        AssertProperty(entity, "Code", required: true, maxLength: 100);
        AssertProperty(entity, "Name", required: true, maxLength: 150);
        AssertProperty(entity, "Category", required: true, maxLength: 50);
        AssertProperty(entity, "DataType", required: true, maxLength: 50);
        AssertProperty(entity, "Description", required: false, maxLength: 250);
        AssertProperty(entity, "IsActive", required: true);
        AssertProperty(entity, "CreatedAt", required: true);
        AssertProperty(entity, "CreatedBy", required: true);
        AssertProperty(entity, "Erased", required: true);

        AssertIndexes(entity, "UX_AbacAttribute_Code", "IX_AbacAttribute_Category_Code");
    }

    [Fact]
    public void Permission_Config_Is_Correct()
    {
        using var context = CreateContext();
        var entity = GetEntityType<Permission>(context);

        AssertTable(entity, "Permissions", "IAM");
        AssertProperty(entity, "Code", required: true, maxLength: 100);
        AssertProperty(entity, "Name", required: true, maxLength: 120);
        AssertProperty(entity, "Description", required: false, maxLength: 250);
        AssertProperty(entity, "IsSystem", required: true);
        AssertProperty(entity, "CreatedAt", required: true);
        AssertProperty(entity, "CreatedBy", required: true);
        AssertProperty(entity, "Erased", required: true);

        AssertIndexes(entity, "UX_Permission_Code");
    }

    [Fact]
    public void Policy_Config_Is_Correct()
    {
        using var context = CreateContext();
        var entity = GetEntityType<Policy>(context);

        AssertTable(entity, "Policies", "IAM");
        AssertProperty(entity, "Code", required: true, maxLength: 100);
        AssertProperty(entity, "Name", required: true, maxLength: 120);
        AssertProperty(entity, "Description", required: false, maxLength: 250);
        AssertProperty(entity, "IsSystem", required: true);
        AssertProperty(entity, "IsActive", required: true);
        AssertProperty(entity, "CreatedAt", required: true);
        AssertProperty(entity, "CreatedBy", required: true);
        AssertProperty(entity, "Erased", required: true);

        AssertIndexes(entity, "UX_Policy_Code");
    }

    [Fact]
    public void Role_Config_Is_Correct()
    {
        using var context = CreateContext();
        var entity = GetEntityType<Role>(context);

        AssertTable(entity, "Roles", "IAM");
        AssertProperty(entity, "Code", required: true, maxLength: 64);
        AssertProperty(entity, "Name", required: true, maxLength: 100);
        AssertProperty(entity, "Description", required: false, maxLength: 250);
        AssertProperty(entity, "IsSystem", required: true);
        AssertProperty(entity, "IsActive", required: true);
        AssertProperty(entity, "CreatedAt", required: true);
        AssertProperty(entity, "CreatedBy", required: true);
        AssertProperty(entity, "Erased", required: true);

        AssertIndexes(entity, "UX_Role_Code");
    }

    [Fact]
    public void User_Config_Is_Correct()
    {
        using var context = CreateContext();
        var entity = GetEntityType<User>(context);

        AssertTable(entity, "Users", "IAM");
        AssertProperty(entity, "ExternalId", required: true, maxLength: 128);
        AssertProperty(entity, "Username", required: true, maxLength: 64);
        AssertProperty(entity, "Email", required: true, maxLength: 256);
        AssertProperty(entity, "DisplayName", required: true, maxLength: 120);
        AssertProperty(entity, "IsActive", required: true);
        AssertProperty(entity, "CreatedAt", required: true);
        AssertProperty(entity, "CreatedBy", required: true);
        AssertProperty(entity, "Erased", required: true);

        AssertIndexes(entity, "UX_User_ExternalId", "UX_User_Username", "UX_User_Email");
    }

    [Fact]
    public void PolicyPermission_Config_Is_Correct()
    {
        using var context = CreateContext();
        var entity = GetEntityType<PolicyPermission>(context);

        AssertTable(entity, "PolicyPermissions", "IAM");
        AssertProperty(entity, "AssignedAt", required: true);
        AssertProperty(entity, "CreatedAt", required: true);
        AssertProperty(entity, "CreatedBy", required: true);
        AssertProperty(entity, "Erased", required: true);

        AssertIndexes(entity, "UX_PolicyPermission_Policy_Permission");
    }

    [Fact]
    public void PolicyRole_Config_Is_Correct()
    {
        using var context = CreateContext();
        var entity = GetEntityType<PolicyRole>(context);

        AssertTable(entity, "PolicyRoles", "IAM");
        AssertProperty(entity, "AssignedAt", required: true);
        AssertProperty(entity, "CreatedAt", required: true);
        AssertProperty(entity, "CreatedBy", required: true);
        AssertProperty(entity, "Erased", required: true);

        AssertIndexes(entity, "UX_PolicyRole_Policy_Role");
    }

    [Fact]
    public void PolicyRule_Config_Is_Correct()
    {
        using var context = CreateContext();
        var entity = GetEntityType<PolicyRule>(context);

        AssertTable(entity, "PolicyRules", "IAM");
        AssertProperty(entity, "Name", required: true, maxLength: 150);
        AssertProperty(entity, "Effect", required: true, maxLength: 10);
        AssertProperty(entity, "Priority", required: true);
        AssertProperty(entity, "IsActive", required: true);
        AssertProperty(entity, "CreatedAt", required: true);
        AssertProperty(entity, "CreatedBy", required: true);
        AssertProperty(entity, "Erased", required: true);

        AssertIndexes(entity, "IX_PolicyRule_Policy_Priority");
    }

    [Fact]
    public void PolicyRuleCondition_Config_Is_Correct()
    {
        using var context = CreateContext();
        var entity = GetEntityType<PolicyRuleCondition>(context);

        AssertTable(entity, "PolicyRuleConditions", "IAM");
        AssertProperty(entity, "Operator", required: true, maxLength: 50);
        AssertProperty(entity, "Value", required: true, maxLength: 500);
        AssertProperty(entity, "ValueType", required: true, maxLength: 30);
        AssertProperty(entity, "IsNegated", required: true);
        AssertProperty(entity, "CreatedAt", required: true);
        AssertProperty(entity, "CreatedBy", required: true);
        AssertProperty(entity, "Erased", required: true);

        AssertIndexes(entity, "IX_PolicyRuleCondition_Rule_Attribute");
    }

    [Fact]
    public void RolePermission_Config_Is_Correct()
    {
        using var context = CreateContext();
        var entity = GetEntityType<RolePermission>(context);

        AssertTable(entity, "RolePermissions", "IAM");
        AssertProperty(entity, "AssignedAt", required: true);
        AssertProperty(entity, "CreatedAt", required: true);
        AssertProperty(entity, "CreatedBy", required: true);
        AssertProperty(entity, "Erased", required: true);

        AssertIndexes(entity, "UX_RolePermission_Role_Permission");
    }

    [Fact]
    public void UserPolicy_Config_Is_Correct()
    {
        using var context = CreateContext();
        var entity = GetEntityType<UserPolicy>(context);

        AssertTable(entity, "UserPolicies", "IAM");
        AssertProperty(entity, "AssignedAt", required: true);
        AssertProperty(entity, "CreatedAt", required: true);
        AssertProperty(entity, "CreatedBy", required: true);
        AssertProperty(entity, "Erased", required: true);

        AssertIndexes(entity, "UX_UserPolicy_User_Policy");
    }

    [Fact]
    public void UserRole_Config_Is_Correct()
    {
        using var context = CreateContext();
        var entity = GetEntityType<UserRole>(context);

        AssertTable(entity, "UserRoles", "IAM");
        AssertProperty(entity, "AssignedAt", required: true);
        AssertProperty(entity, "CreatedAt", required: true);
        AssertProperty(entity, "CreatedBy", required: true);
        AssertProperty(entity, "Erased", required: true);

        AssertIndexes(entity, "UX_UserRole_User_Role");
    }

    private static IEntityType GetEntityType<T>(DbContext context)
    {
        var model = context.GetService<IDesignTimeModel>().Model;
        return model.FindEntityType(typeof(T)) ?? throw new InvalidOperationException($"Missing entity {typeof(T).Name}");
    }

    private static void AssertTable(IEntityType entityType, string table, string schema)
    {
        Assert.Equal(table, entityType.GetTableName());
        Assert.Equal(schema, entityType.GetSchema());
    }

    private static void AssertProperty(IEntityType entityType, string name, bool required, int? maxLength = null)
    {
        var property = entityType.FindProperty(name) ?? throw new InvalidOperationException($"Missing property {name}");
        Assert.Equal(!required, property.IsNullable);
        if (maxLength.HasValue)
        {
            Assert.Equal(maxLength.Value, property.GetMaxLength());
        }
    }

    private static void AssertIndexes(IEntityType entityType, params string[] names)
    {
        var indexNames = entityType.GetIndexes()
            .Select(i => i.GetDatabaseName())
            .Where(n => !string.IsNullOrWhiteSpace(n))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (var name in names)
        {
            Assert.Contains(name, indexNames);
        }
    }
}
