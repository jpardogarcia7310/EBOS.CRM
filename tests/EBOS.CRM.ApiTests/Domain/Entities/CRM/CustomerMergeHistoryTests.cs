using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Exceptions;

namespace EBOS.CRM.ApiTests.Domain.Entities.CRM;

public class CustomerMergeHistoryTests
{
    [Fact]
    public void Create_WithSameWinnerAndMerged_Throws()
    {
        Assert.ThrowsAny<DomainException>(() =>
            CustomerMergeHistory.Create(1, 10, 10, "dedupe", 99));
    }

    [Fact]
    public void Create_WithBlankReason_Throws()
    {
        Assert.ThrowsAny<DomainException>(() =>
            CustomerMergeHistory.Create(1, 10, 20, "   ", 99));
    }

    [Fact]
    public void Create_TrimsReason_AndSetsAuditFields()
    {
        var at = new DateTime(2026, 3, 4, 10, 0, 0, DateTimeKind.Utc);
        var entity = CustomerMergeHistory.Create(1, 10, 20, "  dedupe rule  ", 99, at);

        Assert.Equal(1, entity.TenantId);
        Assert.Equal(10, entity.WinnerCustomerId);
        Assert.Equal(20, entity.MergedCustomerId);
        Assert.Equal("dedupe rule", entity.Reason);
        Assert.Equal(99, entity.CreatedBy);
        Assert.Equal(at, entity.CreatedAt);
    }
}


