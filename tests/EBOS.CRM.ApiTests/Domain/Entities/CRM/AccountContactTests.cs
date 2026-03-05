using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Exceptions;

namespace EBOS.CRM.ApiTests.Domain.Entities.CRM;

public class AccountContactTests
{
    [Fact]
    public void Create_WithValidData_SetsState()
    {
        var start = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var entity = AccountContact.Create(1, 10, 20, true, start, null, 99, start);

        Assert.Equal(1, entity.TenantId);
        Assert.Equal(10, entity.CorporateCustomerId);
        Assert.Equal(20, entity.IndividualCustomerId);
        Assert.True(entity.IsPrimary);
        Assert.Equal(start, entity.StartAt);
        Assert.Null(entity.EndAt);
    }

    [Fact]
    public void Create_WithInvalidTenant_Throws()
    {
        Assert.ThrowsAny<DomainException>(() => AccountContact.Create(0, 10, 20, false, DateTime.UtcNow, null, 1));
    }

    [Fact]
    public void Unassign_WithEndBeforeStart_Throws()
    {
        var start = new DateTime(2026, 1, 10, 0, 0, 0, DateTimeKind.Utc);
        var entity = AccountContact.Create(1, 10, 20, false, start, null, 1);

        Assert.ThrowsAny<DomainException>(() => entity.Unassign(start.AddDays(-1)));
    }

    [Fact]
    public void SetPrimary_WhenUnassigned_Throws()
    {
        var start = new DateTime(2026, 1, 10, 0, 0, 0, DateTimeKind.Utc);
        var entity = AccountContact.Create(1, 10, 20, false, start, null, 1);
        entity.Unassign(start.AddDays(1));

        Assert.ThrowsAny<DomainException>(() => entity.SetPrimary(true));
    }

    [Fact]
    public void ReassignCustomers_WhenUnassigned_Throws()
    {
        var start = new DateTime(2026, 1, 10, 0, 0, 0, DateTimeKind.Utc);
        var entity = AccountContact.Create(1, 10, 20, false, start, null, 1);
        entity.Unassign(start.AddDays(1));

        Assert.ThrowsAny<DomainException>(() => entity.ReassignCustomers(11, 21));
    }
}


