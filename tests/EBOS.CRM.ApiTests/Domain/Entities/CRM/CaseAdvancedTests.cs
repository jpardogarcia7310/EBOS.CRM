using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Exceptions;

namespace EBOS.CRM.ApiTests.Domain.Entities.CRM;

public class CaseAdvancedTests
{
    [Fact]
    public void AssignOwner_WithInvalidId_Throws()
    {
        var entity = BuildCase(Case.StatusOpen);
        Assert.ThrowsAny<DomainException>(() => entity.AssignOwner(0));
    }

    [Fact]
    public void UpdateDueAt_WithDateBeforeCreatedAt_Throws()
    {
        var createdAt = new DateTime(2026, 3, 4, 8, 0, 0, DateTimeKind.Utc);
        var entity = BuildCase(Case.StatusOpen, createdAt);

        Assert.ThrowsAny<DomainException>(() => entity.UpdateDueAt(createdAt.AddMinutes(-1)));
    }

    [Fact]
    public void SetPriority_WithInvalidValue_Throws()
    {
        var entity = BuildCase(Case.StatusOpen);
        Assert.ThrowsAny<DomainException>(() => entity.SetPriority("SUPER_HIGH"));
    }

    [Fact]
    public void Reopen_WhenClosed_SetsReopenedAndClearsClosedAt()
    {
        var entity = BuildCase(Case.StatusResolved);
        entity.Close(DateTime.UtcNow);

        entity.Reopen();

        Assert.Equal(Case.StatusReopened, entity.Status);
        Assert.Null(entity.ClosedAt);
    }

    private static Case BuildCase(string status, DateTime? createdAt = null) => new()
    {
        TenantId = 1,
        Title = "Case",
        Status = status,
        Priority = Case.PriorityLow,
        OwnerUserId = 1,
        QueueId = 1,
        SlaId = 1,
        CreatedAt = createdAt ?? DateTime.UtcNow
    };
}


