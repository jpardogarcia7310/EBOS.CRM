using EBOS.CRM.Domain.Entities.CRM;

namespace EBOS.CRM.ApiTests.Domain.Entities.CRM;

public class SlaAdvancedTests
{
    [Fact]
    public void ValidateWarningMinutes_Throws_WhenNegative()
    {
        var entity = BuildSla(30);
        entity.WarningMinutes = -1;

        Assert.Throws<InvalidOperationException>(() => entity.ValidateWarningMinutes());
    }

    [Fact]
    public void IsBreached_ReturnsFalse_WhenDueAtIsNull()
    {
        var entity = BuildSla(30);
        var result = entity.IsBreached(DateTime.UtcNow, null);
        Assert.False(result);
    }

    [Fact]
    public void IsActiveAt_RespectsBoundaries()
    {
        var now = new DateTime(2026, 3, 4, 10, 0, 0, DateTimeKind.Utc);
        var entity = BuildSla(30);
        entity.ActiveFrom = now.AddMinutes(-1);
        entity.ActiveTo = now.AddMinutes(1);

        Assert.True(entity.IsActiveAt(now));
        Assert.False(entity.IsActiveAt(now.AddMinutes(-2)));
        Assert.False(entity.IsActiveAt(now.AddMinutes(2)));
    }

    private static Sla BuildSla(int targetMinutes) => new()
    {
        TenantId = 1,
        Name = "SLA",
        TargetMinutes = targetMinutes,
        WarningMinutes = 5,
        IsActive = true
    };
}
