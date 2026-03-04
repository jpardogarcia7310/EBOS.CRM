using EBOS.CRM.Application.Shared.Audit;

namespace EBOS.CRM.ApiTests.Application.Shared.Audit;

public class AuditActionsTest
{
    [Fact]
    public void Constants_AreExpected()
    {
        Assert.Equal("Add", AuditActions.Add);
        Assert.Equal("Update", AuditActions.Update);
        Assert.Equal("Patch", AuditActions.Patch);
        Assert.Equal("Delete", AuditActions.Delete);
    }
}
