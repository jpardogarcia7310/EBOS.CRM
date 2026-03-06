using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Events;
using EBOS.CRM.Domain.Exceptions;

namespace EBOS.CRM.ApiTests.Domain.Entities.CRM;

public class AccountContactRoleTests
{
    [Fact]
    public void Create_NormalizesRoleCode_AndSetsPrimary()
    {
        var entity = AccountContactRole.Create(1, 2, " legal_rep ", true, DateTime.UtcNow, null);

        Assert.Equal("LEGAL_REP", entity.RoleCode);
        Assert.True(entity.IsPrimary);
        Assert.Null(entity.ValidTo);
        Assert.Contains(entity.PeekOperationalEvents(), x =>
            x.Name == "AccountContactRoleChanged" &&
            x.Category == DomainOperationalEventCategory.Business);
    }

    [Fact]
    public void Create_WithInvalidRoleCode_Throws()
    {
        Assert.ThrowsAny<DomainException>(() =>
            AccountContactRole.Create(1, 2, "   ", true, DateTime.UtcNow, null));
    }

    [Fact]
    public void Deactivate_WithValidToBeforeValidFrom_Throws()
    {
        var validFrom = new DateTime(2026, 1, 10, 0, 0, 0, DateTimeKind.Utc);
        var entity = AccountContactRole.Create(1, 2, "LEGAL_REP", false, validFrom, null);

        Assert.ThrowsAny<DomainException>(() => entity.Deactivate(validFrom.AddMinutes(-1)));
    }

    [Fact]
    public void SetPrimary_WhenInactive_Throws()
    {
        var validFrom = new DateTime(2026, 1, 10, 0, 0, 0, DateTimeKind.Utc);
        var entity = AccountContactRole.Create(1, 2, "LEGAL_REP", false, validFrom, validFrom.AddDays(1));

        Assert.ThrowsAny<DomainException>(() => entity.SetPrimary(true));
        Assert.Contains(entity.PeekOperationalEvents(), x =>
            x.Name == "DomainInvariantBreachDetected" &&
            x.Category == DomainOperationalEventCategory.Anomaly);
    }

    [Fact]
    public void SetPrimary_SameValue_IsIdempotentAndEmitsTechnicalDedupEvent()
    {
        var validFrom = new DateTime(2026, 1, 10, 0, 0, 0, DateTimeKind.Utc);
        var entity = AccountContactRole.Create(1, 2, "LEGAL_REP", false, validFrom, null);

        entity.SetPrimary(false);

        Assert.Contains(entity.PeekOperationalEvents(), x =>
            x.Name == "DomainCommandDeduplicated" &&
            x.Category == DomainOperationalEventCategory.Technical);
    }
}


