using EBOS.CRM.Domain.Entities;
using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EBOS.CRM.ApiTests.Infrastructure.Persistence;

public class EbosEntityConfigurationTests
{
    private static CrmDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<CrmDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
            .Options;
        return new CrmDbContext(options);
    }

    [Fact]
    public void AddressType_Config_Is_Correct()
    {
        using var context = CreateContext();
        var entity = GetEntityType<AddressType>(context);

        AssertTable(entity, "AddressTypes", "EBOS");
        AssertProperty(entity, "Code", required: true, maxLength: 50);
        AssertProperty(entity, "Description", required: true, maxLength: 200);
        AssertProperty(entity, "Category", required: false, maxLength: 50);
        AssertProperty(entity, "AllowsMultiple", required: true);
        AssertProperty(entity, "RequiresPrimary", required: true);
        AssertProperty(entity, "CreatedAt", required: true);
        AssertProperty(entity, "CreatedBy", required: true);
    }

    [Fact]
    public void Country_Config_Is_Correct()
    {
        using var context = CreateContext();
        var entity = GetEntityType<Country>(context);

        AssertTable(entity, "Countries", "EBOS");
        AssertProperty(entity, "Name", required: true, maxLength: 200);
        AssertProperty(entity, "Iso31661A2Code", required: true, maxLength: 2);
        AssertProperty(entity, "Iso31661A3Code", required: true, maxLength: 3);
        AssertProperty(entity, "Iso31661NumCode", required: true, maxLength: 10);
        AssertProperty(entity, "Domain", required: true, maxLength: 5);
        AssertProperty(entity, "Currency", required: true, maxLength: 100);
        AssertProperty(entity, "CurrencyCode", required: true, maxLength: 10);
        AssertProperty(entity, "InternationalPhoneCode", required: true, maxLength: 20);
        AssertProperty(entity, "CreatedAt", required: true);
        AssertProperty(entity, "CreatedBy", required: true);

        AssertIndexes(entity, "IX_Countries_Iso31661A2Code", "IX_Countries_Iso31661A3Code",
            "IX_Countries_Iso31661NumCode", "IX_Countries_Name", "IX_Countries_Domain",
            "IX_Countries_CurrencyCode");
        AssertCheckConstraints(entity, "CK_Countries_IsoA2_Length", "CK_Countries_IsoA3_Length",
            "CK_Country_IsoA2_Uppercase", "CK_Country_IsoA3_Uppercase", "CK_Country_IsoNum_Digits");
    }

    [Fact]
    public void IdentificationType_Config_Is_Correct()
    {
        using var context = CreateContext();
        var entity = GetEntityType<IdentificationType>(context);

        AssertTable(entity, "IdentificationTypes", "EBOS");
        AssertProperty(entity, "Code", required: true, maxLength: 50);
        AssertProperty(entity, "Description", required: true, maxLength: 200);
        AssertProperty(entity, "CreatedAt", required: true);
        AssertProperty(entity, "CreatedBy", required: true);
        AssertProperty(entity, "Erased", required: true);
    }

    [Fact]
    public void Status_Config_Is_Correct()
    {
        using var context = CreateContext();
        var entity = GetEntityType<Status>(context);

        AssertTable(entity, "Statuses", "EBOS");
        AssertProperty(entity, "Description", required: true, maxLength: 100);
        AssertProperty(entity, "CreatedAt", required: true);
        AssertProperty(entity, "CreatedBy", required: true);
    }

    [Fact]
    public void TenantConfiguration_Config_Is_Correct()
    {
        using var context = CreateContext();
        var entity = GetEntityType<TenantConfiguration>(context);

        AssertTable(entity, "TenantConfigurations", "EBOS");
        AssertProperty(entity, "TenantId", required: true);
        AssertProperty(entity, "Key", required: true, maxLength: 200);
        AssertProperty(entity, "ValueJson", required: true, maxLength: 4000);
        AssertProperty(entity, "UpdatedAt", required: true);
        AssertProperty(entity, "UpdatedBy", required: true);
        AssertProperty(entity, "Erased", required: true);

        AssertIndexes(entity, "IX_TenantConfiguration_TenantId", "UX_TenantConfiguration_TenantId_Key");
    }

    [Fact]
    public void TenantQuota_Config_Is_Correct()
    {
        using var context = CreateContext();
        var entity = GetEntityType<TenantQuota>(context);

        AssertTable(entity, "TenantQuotas", "EBOS");
        AssertProperty(entity, "TenantId", required: true);
        AssertProperty(entity, "Metric", required: true, maxLength: 100);
        AssertPrecision(entity, "Limit", 18, 4, required: true);
        AssertProperty(entity, "Unit", required: false, maxLength: 20);
        AssertProperty(entity, "EffectiveFrom", required: true);
        AssertProperty(entity, "Erased", required: true);

        AssertIndexes(entity, "IX_TenantQuota_TenantId", "UX_TenantQuota_TenantId_Metric_EffectiveFrom");
    }

    [Fact]
    public void TenantUsageMetric_Config_Is_Correct()
    {
        using var context = CreateContext();
        var entity = GetEntityType<TenantUsageMetric>(context);

        AssertTable(entity, "TenantUsageMetrics", "EBOS");
        AssertProperty(entity, "TenantId", required: true);
        AssertProperty(entity, "Metric", required: true, maxLength: 100);
        AssertPrecision(entity, "Value", 18, 4, required: true);
        AssertProperty(entity, "Unit", required: false, maxLength: 20);
        AssertProperty(entity, "PeriodStart", required: true);
        AssertProperty(entity, "PeriodEnd", required: true);
        AssertProperty(entity, "Source", required: false, maxLength: 100);
        AssertProperty(entity, "Erased", required: true);

        AssertIndexes(entity, "IX_TenantUsageMetric_TenantId", "IX_TenantUsageMetric_TenantId_Metric_PeriodStart");
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

    private static void AssertPrecision(IEntityType entityType, string name, int precision, int scale, bool required = false)
    {
        var property = entityType.FindProperty(name) ?? throw new InvalidOperationException($"Missing property {name}");
        if (required)
        {
            Assert.False(property.IsNullable);
        }
        Assert.Equal(precision, property.GetPrecision());
        Assert.Equal(scale, property.GetScale());
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

    private static void AssertCheckConstraints(IEntityType entityType, params string[] names)
    {
        var constraintNames = entityType.GetCheckConstraints()
            .Select(c => c.Name)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (var name in names)
        {
            Assert.Contains(name, constraintNames);
        }
    }
}
