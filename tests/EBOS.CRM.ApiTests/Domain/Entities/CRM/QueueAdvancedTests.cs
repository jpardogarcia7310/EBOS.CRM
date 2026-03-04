using EBOS.CRM.Domain.Entities.CRM;

namespace EBOS.CRM.ApiTests.Domain.Entities.CRM;

public class QueueAdvancedTests
{
    [Fact]
    public void Queue_CasesCollection_IsInitialized()
    {
        var entity = new Queue();
        Assert.NotNull(entity.Cases);
        Assert.Empty(entity.Cases);
    }
}
